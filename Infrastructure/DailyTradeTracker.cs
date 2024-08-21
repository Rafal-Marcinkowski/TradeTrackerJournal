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

    public DailyTradeTracker(ITransactionData transactionData, IDailyDataProvider dailyDataProvider, IEventAggregator eventAggregator)
    {
        this.transactionData = transactionData;
        this.dailyDataProvider = dailyDataProvider;
        this.eventAggregator = eventAggregator;
    }

    public async Task StartTracker()
    {
        Log.Information("Rozpoczynam śledzenie transakcji.");
        var transactions = await transactionData.GetAllTransactionsAsync();

        if (transactions.Any())
        {
            transactions = transactions.Where(q => q.IsTracking);
            await TrackTransactions(transactions);
        }
    }

    private async Task TrackTransactions(IEnumerable<Transaction> transactions)
    {
        Log.Information("Śledzenie transakcji");
        foreach (var transaction in transactions)
        {
            bool isHistoricalTransaction = await AssessTransaction(transaction.EntryDate.Date);
            if (isHistoricalTransaction)
            {
                await TrackArchivedTransaction(transaction);
            }
            else
            {
                await TrackRecentTransaction(transaction);
            }

            await CheckTrackingProgress(transaction);
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

        if (newRecords.Any())
        {
            foreach (var record in newRecords)
            {
                var dailyData = await GetDailyData.GetAsync(record);
                dailyData.TransactionID = transaction.ID;
                dailyData.TurnoverChange = await CalculateDailyDataProperties.CalculateTurnoverChange(transaction.EntryMedianTurnover, dailyData.Turnover);
                dailyData.PriceChange = await CalculateDailyDataProperties.CalculatePriceChange(transaction.EntryPrice, dailyData.ClosePrice);
                var checkDailyData = new CheckDailyData(transactionData, dailyDataProvider);
                if (!await checkDailyData.IsExisting(dailyData))
                {
                    await dailyDataProvider.InsertDailyDataAsync(dailyData);
                    eventAggregator.GetEvent<DailyDataAddedEvent>().Publish(dailyData);
                }
            }
        }
    }

    private async Task TrackArchivedTransaction(Transaction transaction)
    {
        var dailyDataCollection = await dailyDataProvider.GetDailyDataForTransactionAsync(transaction.ID);

        var records = await GetDataRecords.PrepareDataRecords(transaction, false);

        var recordsBeforeTransaction = records
           .Where(r => r.Date <= transaction.EntryDate.Date)
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
            var additionalRecords = await GetDataRecords.GetAdditionalRecords(transaction, remainingBefore, beforeTransaction: true);
            recordsBeforeTransaction.InsertRange(0, additionalRecords);
        }

        decimal archivedEntryTurnoverMedian = CalculateArchivedTurnoverMedian.Calculate(recordsBeforeTransaction);
        transaction.EntryMedianTurnover = (int)archivedEntryTurnoverMedian;
        await transactionData.UpdateTransactionAsync(transaction);

        await InsertData(transaction, recordsAfterTransaction);
    }

    private async Task InsertData(Transaction transaction, List<DataRecord> records)
    {
        foreach (var record in records)
        {
            DailyData dailyData = await GetDailyData.GetAsync(record);
            dailyData.TurnoverChange = await CalculateDailyDataProperties.CalculateTurnoverChange(transaction.EntryMedianTurnover, dailyData.Turnover);
            dailyData.PriceChange = await CalculateDailyDataProperties.CalculatePriceChange(transaction.EntryPrice, dailyData.ClosePrice);
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
