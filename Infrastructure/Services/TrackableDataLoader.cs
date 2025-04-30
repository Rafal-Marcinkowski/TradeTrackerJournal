using Infrastructure.Interfaces;
using SharedProject.Interfaces;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Infrastructure.Services;

public class TrackableDataLoader(ITradeTrackerFacade facade)
{
    public async Task OnTrackableUpdated<T>(T item, ObservableCollection<T> items) where T : ITrackable
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var existingItem = items.FirstOrDefault(x => x.ID == item.ID);
            if (existingItem != null)
            {
                existingItem.EntryMedianTurnover = item.EntryMedianTurnover;
            }
        });
    }

    public async Task OnDailyDataAdded<T>(DailyData dailyData, ObservableCollection<T> items) where T : ITrackable
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var item = items.FirstOrDefault(x =>
                (x is Transaction tx && dailyData.TransactionID.HasValue && tx.ID == dailyData.TransactionID.Value) ||
                (x is Event ev && dailyData.EventID.HasValue && ev.ID == dailyData.EventID.Value));

            if (item != null)
            {
                if (item is Transaction tx)
                {
                    dailyData.TransactionCloseDate = tx.CloseDate;
                    dailyData.TransactionClosingDescription = tx.ClosingDescription;
                }
                item.DailyDataCollection.Add(dailyData);
            }
        });
    }

    public async Task<ObservableCollection<T>> GetLastXItems<T>(int count) where T : ITrackable
    {
        var items = await GetAllItems<T>();
        return await LoadDetails(items.OrderByDescending(q => q.EntryDate).Take(count).ToList());
    }

    public async Task<ObservableCollection<T>> GetAllOpenItems<T>() where T : ITrackable
    {
        var items = await GetAllItems<T>();
        return await LoadDetails(items.Where(q => !q.IsClosed).OrderByDescending(q => q.EntryDate).ToList());
    }

    public async Task<ObservableCollection<T>> GetItemsForCompany<T>(int companyId) where T : ITrackable
    {
        var items = await GetAllItems<T>();
        return await LoadDetails(items.Where(q => q.CompanyID == companyId)
            .OrderByDescending(q => q.EntryDate).ToList());
    }

    private async Task<List<T>> GetAllItems<T>() where T : ITrackable
    {
        if (typeof(T) == typeof(Transaction))
        {
            return [.. (await facade.TransactionManager.GetAllTransactions()).Cast<T>()];
        }
        else if (typeof(T) == typeof(Event))
        {
            return [.. (await facade.EventManager.GetAllEvents()).Cast<T>()];
        }
        throw new NotSupportedException($"Type {typeof(T)} is not supported");
    }

    public async Task<ObservableCollection<Event>> GetEventsForTransaction(int transactionId)
    {
        var tx = await facade.TransactionManager.GetTransaction(transactionId);
        var events = (await facade.EventManager.GetEventsForCompany(tx.CompanyID))
            .Where(e => e.EntryDate >= tx.EntryDate)
            .OrderByDescending(e => e.EntryDate)
            .ToList();
        return await LoadDetails(events);
    }

    private async Task<ObservableCollection<T>> LoadDetails<T>(List<T> items) where T : ITrackable
    {
        foreach (var item in items)
        {
            if (item is Transaction tx)
            {
                var dailyData = (await facade.DailyDataProvider.GetDailyDataForTransactionAsync(tx.ID))
                    .OrderBy(q => q.Date).ToList();

                var allEvents = await facade.EventManager.GetEventsForCompany(tx.CompanyID);

                foreach (var data in dailyData)
                {
                    var match = allEvents.FirstOrDefault(q => q.EntryDate.Date == data.Date.Date);
                    data.EventDate = match?.EntryDate.Date;
                    data.TransactionCloseDate = tx.CloseDate;
                    data.TransactionClosingDescription = tx.ClosingDescription;
                }

                tx.DailyDataCollection = [.. dailyData];
                tx.Comments = [.. await facade.CommentManager.GetCommentsForTransaction(tx.ID)];
            }
            else if (item is Event ev)
            {
                var dailyData = (await facade.DailyDataProvider.GetDailyDataForEventAsync(ev.ID))
                    .OrderBy(q => q.Date).ToList();

                ev.DailyDataCollection = [.. dailyData];
                ev.Comments = [.. await facade.CommentManager.GetCommentsForEvent(ev.ID)];
            }
        }
        return [.. items];
    }
}