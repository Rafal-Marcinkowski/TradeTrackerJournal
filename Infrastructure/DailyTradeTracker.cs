using DataAccess.Data;
using Infrastructure.Calculations;
using Infrastructure.Events;
using Infrastructure.GetDataFromHtml;
using Prism.Events;
using Serilog;
using SharedModels.Models;

namespace Infrastructure;

public class DailyTradeTracker
{
    private readonly ITransactionData transactionData;
    private readonly IDailyDataProvider dailyDataProvider;
    private readonly IEventAggregator eventAggregator;
    private List<Transaction> failedTransactions;
    private static bool isTrackerWorking = false;
    private Queue<Transaction> queuedTransactions = [];

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
        if (!isTrackerWorking)
        {
            await StartTracker(transaction);
        }

        else
        {
            queuedTransactions.Enqueue(transaction);
        }
    }

    public async Task StartTracker(Transaction transaction = null)
    {

        isTrackerWorking = true;
        Log.Information("Rozpoczynam śledzenie.");

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
            Log.Information($"{transactions.Count} transakcji do prześledzenia.");
            await TrackTransactions(transactions);
            await ProcessQueuedTransactions();
        }

        isTrackerWorking = false;
        Log.Information("Koniec śledzenia transakcji.");
    }

    private async Task ProcessQueuedTransactions()
    {
        while (queuedTransactions.Count > 0)
        {
            var nextTransaction = queuedTransactions.Dequeue();
            await StartTracker(nextTransaction);
        }
    }

    private async Task TrackTransactions(IEnumerable<Transaction> transactions)
    {
        try
        {
            foreach (var transaction in transactions)
            {
                Log.Information($"Rozpoczynam śledzenie transkacji ID: {transaction.ID}, {transaction.CompanyName}");

                await Track(transaction);

                Log.Information($"Śledzenie transkacji ID: {transaction.ID}, {transaction.CompanyName} zakończone.");
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
        Log.Information($"Liczba rekordów pobranych z zewnętrznego źródła: {allRecords.Count()}");
        if (!allRecords.Any())
        {
            return;
        }

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
            .Take(30 - currentDailyData.Count())
            .ToList();

        if (recordsToAdd.Count != 0)
        {
            foreach (var item in recordsToAdd)
            {
                item.TransactionID = transaction.ID;
                item.PriceChange = await DailyDataProperties.CalculatePriceChange(transaction.EntryPrice, item.ClosePrice);
                item.TurnoverChange = await DailyDataProperties.CalculateTurnoverChange(transaction.EntryMedianTurnover, item.Turnover);
                Log.Information($"Dodawanie rekordu: Date={item.Date}, TransactionID={item.TransactionID}, ClosePrice={item.ClosePrice}, Turnover={item.Turnover}, PriceChange, {item.PriceChange}, TurnoverChange={item.TurnoverChange}");
                try
                {
                    await dailyDataProvider.InsertDailyDataAsync(item);
                }
                catch (Exception ex)
                {
                    Log.Error<Exception>(ex.Message, ex);
                }
                eventAggregator.GetEvent<DailyDataAddedEvent>().Publish(item);
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
}
