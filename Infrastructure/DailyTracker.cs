using DataAccess.Data;
using Infrastructure.Calculations;
using Infrastructure.Events;
using Infrastructure.GetDataFromHtml;
using Serilog;
using SharedProject.Interfaces;
using SharedProject.Models;

namespace Infrastructure;

public class DailyTracker
{
    private readonly ITransactionData transactionData;
    private readonly IDailyDataProvider dailyDataProvider;
    private readonly IEventAggregator eventAggregator;
    private readonly IEventData eventData;
    private static bool isTrackerWorking = false;
    private readonly Queue<ITrackable> queuedTrackables = [];

    public DailyTracker(ITransactionData transactionData, IDailyDataProvider dailyDataProvider, IEventAggregator eventAggregator, IEventData eventData)
    {
        this.transactionData = transactionData;
        this.eventData = eventData;
        this.dailyDataProvider = dailyDataProvider;
        this.eventAggregator = eventAggregator;

        this.eventAggregator.GetEvent<TransactionAddedEvent>().Subscribe(async transaction =>
        {
            await OnTrackableAdded(transaction);
        });

        this.eventAggregator.GetEvent<EventAddedEvent>().Subscribe(async e =>
        {
            await OnTrackableAdded(e);
        });
    }

    private async Task OnTrackableAdded(ITrackable trackable)
    {
        if (!isTrackerWorking)
        {
            await StartTracker(trackable);
        }

        else
        {
            queuedTrackables.Enqueue(trackable);
        }
    }

    public async Task StartTracker(ITrackable trackable = null)
    {

        isTrackerWorking = true;
        Log.Information("Rozpoczynam śledzenie.");

        List<ITrackable> trackables = [];

        if (trackable is not null)
        {
            trackables.Add(trackable);
        }

        else
        {
            var transactions = await transactionData.GetAllTransactionsAsync();
            var events = await eventData.GetAllEventsAsync();
            trackables.AddRange(transactions);
            trackables.AddRange(events);
        }

        if (trackables.Count != 0)
        {
            trackables = [.. trackables.Where(q => q.IsTracking)];
            Log.Information($"{trackables.Count} transakcji/zdarzeń do prześledzenia.");
            await TrackTrackables(trackables);
            await ProcessQueuedTrackables();
        }

        isTrackerWorking = false;
        Log.Information("Koniec śledzenia transakcji/zdarzeń.");
    }

    private async Task ProcessQueuedTrackables()
    {
        while (queuedTrackables.Count > 0)
        {
            var nextTrackable = queuedTrackables.Dequeue();
            await StartTracker(nextTrackable);
        }
    }

    private async Task TrackTrackables(IEnumerable<ITrackable> trackables)
    {
        try
        {
            foreach (var trackable in trackables)
            {
                Log.Information($"Rozpoczynam śledzenie obiektu ID: {trackable.ID}, {trackable.CompanyName}");

                await Track(trackable);

                Log.Information($"Śledzenie obiektu ID: {trackable.ID}, {trackable.CompanyName} zakończone.");
            }
        }
        catch (Exception ex)
        {
            Log.Error<Exception>(ex.Message, ex);
        }
    }

    private async Task Track(ITrackable trackable)
    {
        var allRecords = await GetDataRecords.GetAllNecessaryRecords(trackable);
        Log.Information($"Liczba rekordów pobranych z zewnętrznego źródła: {allRecords.Count()}");

        if (!allRecords.Any())
        {
            return;
        }

        if (trackable.EntryMedianTurnover == 0)
        {
            var medianRecords = allRecords
                .Where(q => q.Date < trackable.EntryDate.Date)
                .OrderByDescending(q => q.Date)
                .Take(20).ToList();
            trackable.EntryMedianTurnover = (int)await ArchivedTurnoverMedian.Calculate(medianRecords);

            switch (trackable)
            {
                case Transaction transaction:
                    await transactionData.UpdateTransactionAsync(transaction);
                    eventAggregator.GetEvent<TransactionUpdatedEvent>().Publish(transaction);
                    break;

                case Event e:
                    await eventData.UpdateEventAsync(e);
                    eventAggregator.GetEvent<EventUpdatedEvent>().Publish(e);
                    break;
            }
        }

        var trackingRecords = allRecords
                .Where(q => q.Date >= trackable.EntryDate.Date)
                .OrderBy(q => q.Date)
                .Take(trackable.IsClosed ? 30 : allRecords.Count()).ToList();

        if (trackingRecords.Count != 0)
        {
            await UpdateDailyDataEntries(trackingRecords, trackable);
        }
    }

    private async Task UpdateDailyDataEntries(IEnumerable<DataRecord> trackingRecords, ITrackable trackable)
    {
        var newDataList = new List<DailyData>();

        foreach (var record in trackingRecords)
        {
            var dailyData = await PrepareDailyData.GetAsync(record);
            newDataList.Add(dailyData);
        }

        IEnumerable<DailyData> currentDailyData = trackable switch
        {
            Transaction transaction => await dailyDataProvider.GetDailyDataForTransactionAsync(transaction.ID),
            Event e => await dailyDataProvider.GetDailyDataForEventAsync(e.ID),
            _ => []
        };

        var oldestCurrentRecord = currentDailyData.OrderByDescending(d => d.Date).FirstOrDefault();

        var recordsToAdd = newDataList
            .Where(d => oldestCurrentRecord == null || d.Date > oldestCurrentRecord.Date)
            .OrderBy(d => d.Date)
            .Take(trackable.IsClosed ? 30 - currentDailyData.Count() : newDataList.Count)
            .ToList();

        if (recordsToAdd.Count != 0)
        {
            foreach (var item in recordsToAdd)
            {
                switch (trackable)
                {
                    case Transaction transaction:
                        item.TransactionID = transaction.ID;
                        item.EventID = null;
                        break;

                    case Event evt:
                        item.TransactionID = null;
                        item.EventID = evt.ID;
                        break;
                }

                item.PriceChange = await DailyDataProperties.CalculatePriceChange(trackable.EntryPrice, item.ClosePrice);
                item.TurnoverChange = await DailyDataProperties.CalculateTurnoverChange(trackable.EntryMedianTurnover, item.Turnover);

                Log.Information($"Dodawanie rekordu: Date={item.Date}, TransactionID={item.TransactionID}, EventID={item.EventID}, " +
                                $"ClosePrice={item.ClosePrice}, Turnover={item.Turnover}, PriceChange={item.PriceChange}, TurnoverChange={item.TurnoverChange}");

                await dailyDataProvider.InsertDailyDataAsync(item);

                eventAggregator.GetEvent<DailyDataAddedEvent>().Publish(item);
            }

            await CheckTrackingProgress(trackable);
        }
    }

    private async Task CheckTrackingProgress(ITrackable trackable)
    {
        IEnumerable<DailyData> dailyDataCollection = trackable switch
        {
            Transaction transaction when transaction.IsClosed =>
                await dailyDataProvider.GetDailyDataForTransactionAsync(trackable.ID),
            Event _ =>
                await dailyDataProvider.GetDailyDataForEventAsync(trackable.ID),
            _ => []
        };

        if (!dailyDataCollection.Any() || dailyDataCollection.Count() < 30)
        {
            return;
        }

        await UpdateTrackingStatus(trackable);
    }

    private async Task UpdateTrackingStatus(ITrackable trackable)
    {
        switch (trackable)
        {
            case Transaction:
                Transaction transaction = await transactionData.GetTransactionAsync(trackable.ID);
                transaction.IsTracking = false;
                await transactionData.UpdateTransactionAsync(transaction);
                break;

            case Event:
                Event trackableEvent = await eventData.GetEventAsync(trackable.ID);
                trackableEvent.IsTracking = false;
                await eventData.UpdateEventAsync(trackableEvent);
                break;
        }
    }
}
