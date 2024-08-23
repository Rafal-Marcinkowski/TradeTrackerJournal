using DataAccess.Data;
using Infrastructure.Calculations;
using Infrastructure.Events;
using Infrastructure.GetDataFromHtml;
using Prism.Events;
using Serilog;
using SharedModels.Models;
using ValidationComponent.DailyDataValidation;

namespace Infrastructure;

public class DailyTradeTracker
{
    private readonly ITransactionData transactionData;
    private readonly IDailyDataProvider dailyDataProvider;
    private readonly IEventAggregator eventAggregator;
    private List<Transaction> failedTransactions;
    private SemaphoreSlim semaphore = new(1, 1);
    public static bool isTrackerWorking = false;

    public DailyTradeTracker(ITransactionData transactionData, IDailyDataProvider dailyDataProvider, IEventAggregator eventAggregator)
    {
        this.transactionData = transactionData;
        this.dailyDataProvider = dailyDataProvider;
        this.eventAggregator = eventAggregator;
        this.eventAggregator.GetEvent<TransactionAddedEvent>().Subscribe(async transaction =>
        {
            await OnTransactionAdded(transaction);
        });
    }

    private async Task OnTransactionAdded(Transaction transaction)
    {
        await semaphore.WaitAsync();

        try
        {
            if (!isTrackerWorking)
            {
                await StartTracker(transaction);
            }
            else
            {
                await semaphore.WaitAsync();
                await StartTracker(transaction);
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task StartTracker(Transaction transaction = null)
    {
        Log.Information("Rozpoczynam śledzenie.");
        isTrackerWorking = true;
        List<Transaction> transactions = [];

        if (transaction is not null)
        {
            transactions.Add(transaction);
        }
        else
        {
            transactions = (List<Transaction>)await transactionData.GetAllTransactionsAsync();
        }

        if (transactions.Count != 0)
        {
            transactions = transactions.Where(q => q.IsTracking).ToList();
            await TrackTransactions(transactions);
        }
        isTrackerWorking = false;
    }

    private async Task TrackTransactions(IEnumerable<Transaction> transactions)
    {
        try
        {
            foreach (var transaction in transactions)
            {
                Log.Information($"Śledzenie transkacji ID: {transaction.ID}, {transaction.CompanyName}");

                await Track(transaction);

                await CheckTrackingProgress(transaction);
            }
        }
        catch (Exception ex)
        {
            Log.Error<Exception>(ex.Message, ex);
        }
    }

    private async Task Track(Transaction transaction)
    {
        var allRecords = await GetDataRecords.GetAllNecessaryRecords(transaction);

        if (transaction.EntryMedianTurnover == 0)
        {
            var medianRecords = allRecords
                .Where(q => q.Date < transaction.EntryDate.Date)
                .OrderByDescending(q => q.Date)
                .Take(20).ToList();
            transaction.EntryMedianTurnover = (int)await ArchivedTurnoverMedian.Calculate(medianRecords);
            await transactionData.UpdateTransactionAsync(transaction);
            eventAggregator.GetEvent<TransactionUpdatedEvent>().Publish(transaction);
        }

        var trackingRecords = allRecords
                .Where(q => q.Date >= transaction.EntryDate.Date)
                .OrderBy(q => q.Date)
                .Take(30).ToList();

        if (trackingRecords.Count != 0)
        {
            await UpdateDailyDataEntries(trackingRecords, transaction);
        }
    }

    private async Task UpdateDailyDataEntries(IEnumerable<DataRecord> trackingRecords, Transaction transaction)
    {
        List<DailyData> newDataList = [];
        foreach (var record in trackingRecords)
        {
            DailyData dailyData = await PrepareDailyData.GetAsync(record);
            newDataList.Add(dailyData);
        }
        var currentDailyData = await dailyDataProvider.GetDailyDataForTransactionAsync(transaction.ID);
        var oldestCurrentRecord = currentDailyData.OrderBy(d => d.Date).FirstOrDefault();

        var recordsToAdd = newDataList
            .Where(d => oldestCurrentRecord == null || d.Date > oldestCurrentRecord.Date)
            .OrderBy(d => d.Date)
            .Take(30)
            .ToList();

        if (recordsToAdd.Count != 0)
        {
            foreach (var item in recordsToAdd)
            {
                item.TransactionID = transaction.ID;
                item.PriceChange = await DailyDataProperties.CalculatePriceChange(transaction.EntryPrice, item.ClosePrice);
                item.TurnoverChange = await DailyDataProperties.CalculateTurnoverChange(transaction.EntryMedianTurnover, item.Turnover);
                await dailyDataProvider.InsertDailyDataAsync(item);
                eventAggregator.GetEvent<DailyDataAddedEvent>().Publish(item);
            }
        }
    }

    private async Task CheckTrackingProgress(Transaction transaction)
    {
        var dailyDataCollection = await dailyDataProvider.GetDailyDataForTransactionAsync(transaction.ID);
        if (dailyDataCollection.Any())
        {
            if (dailyDataCollection.Count() >= 30)
            {
                transaction.IsTracking = false;
                await transactionData.UpdateTransactionAsync(transaction);
            }
        }
    }

    private async Task TrackRecentTransaction(Transaction transaction)
    {
        Log.Information($"Śledzenie świeżej transakcji {transaction.ID}, {transaction.CompanyName}");
        try
        {
            var dailyDataCollection = await dailyDataProvider.GetDailyDataForTransactionAsync(transaction.ID);
            DateTime lastUpdateDate;

            if (dailyDataCollection.Any())
            {
                lastUpdateDate = dailyDataCollection.OrderByDescending(d => d.Date).First().Date.Date;
            }
            else
            {
                lastUpdateDate = transaction.EntryDate.Date;
            }

            var records = await GetDataRecords.PrepareDataRecords(transaction);
            var newRecords = records
               .Where(r => r.Date.Date >= lastUpdateDate)
               .OrderBy(r => r.Date)
               .ToList();

            if (newRecords.Count != 0)
            {
                foreach (var record in newRecords)
                {
                    var dailyData = await PrepareDailyData.GetAsync(record);
                    dailyData.TransactionID = transaction.ID;
                    dailyData.TurnoverChange = await DailyDataProperties.CalculateTurnoverChange(transaction.EntryMedianTurnover, dailyData.Turnover);
                    dailyData.PriceChange = await DailyDataProperties.CalculatePriceChange(transaction.EntryPrice, dailyData.ClosePrice);
                    var checkDailyData = new CheckDailyData(transactionData, dailyDataProvider);
                    if (!await checkDailyData.IsExisting(dailyData))
                    {
                        await dailyDataProvider.InsertDailyDataAsync(dailyData);
                        eventAggregator.GetEvent<DailyDataAddedEvent>().Publish(dailyData);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error<Exception>(ex.Message, ex);
        }
    }

    private async Task TrackArchivedTransaction(Transaction transaction)
    {
        Log.Information($"Śledzenie starej transakcji {transaction.ID}, {transaction.CompanyName}");
        try
        {
            var dailyDataCollection = await dailyDataProvider.GetDailyDataForTransactionAsync(transaction.ID);

            var records = await GetDataRecords.PrepareDataRecords(transaction, false);
            if (!records.Any())
            {
                Log.Error<Transaction>($"Problemy przy śledzeniu: {transaction.ID}, {transaction.CompanyName}", transaction);
                return;
            }

            var recordsBeforeTransaction = records
               .Where(r => r.Date < transaction.EntryDate.Date)
               .OrderByDescending(r => r.Date)
               .Take(20)
               .ToList();

            var recordsAfterTransaction = records
               .Where(r => r.Date >= transaction.EntryDate.Date)
               .OrderBy(r => r.Date)
               .Take(30)
               .ToList();

            int remainingBefore = 20 - recordsBeforeTransaction.Count;
            if (remainingBefore > 0)
            {
                var additionalRecords = await GetDataRecords.GetAdditionalRecords(transaction.CompanyName, remainingBefore, beforeTransaction: true, transaction.EntryDate);
                recordsBeforeTransaction.InsertRange(0, additionalRecords);
            }

            await InsertData(transaction, recordsAfterTransaction);
        }
        catch (Exception ex)
        {
            Log.Error<Exception>(ex.Message, ex);
        }
    }

    private async Task InsertData(Transaction transaction, List<DataRecord> records)
    {
        foreach (var record in records)
        {
            DailyData dailyData = await PrepareDailyData.GetAsync(record);
            dailyData.TurnoverChange = await DailyDataProperties.CalculateTurnoverChange(transaction.EntryMedianTurnover, dailyData.Turnover);
            dailyData.PriceChange = await DailyDataProperties.CalculatePriceChange(transaction.EntryPrice, dailyData.ClosePrice);
            dailyData.TransactionID = transaction.ID;
            await dailyDataProvider.InsertDailyDataAsync(dailyData);
            eventAggregator.GetEvent<DailyDataAddedEvent>().Publish(dailyData);
        }
    }

    private async Task<bool> AssessTransaction(DateTime date)
    {
        TimeSpan timeSpan = DateTime.Now.Date - date;
        if (timeSpan.TotalDays >= 30)
        {
            return true;
        }
        return false;
    }
}
