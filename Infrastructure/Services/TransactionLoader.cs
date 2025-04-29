using Infrastructure.Interfaces;
using SharedProject.Models;
using System.Collections.ObjectModel;
using System.Windows;

namespace Infrastructure.Services;

public class TransactionLoader(ITradeTrackerFacade facade)
{
    public async Task OnTransactionUpdated(Transaction transaction, ObservableCollection<Transaction> transactions)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var t = transactions.FirstOrDefault(x => x.ID == transaction.ID);
            if (t != null) t.EntryMedianTurnover = transaction.EntryMedianTurnover;
        });
    }

    public async Task OnDailyDataAdded(DailyData dailyData, ObservableCollection<Transaction> transactions)
    {
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            var tx = transactions.FirstOrDefault(x => x.ID == dailyData.TransactionID);
            if (tx != null)
            {
                dailyData.TransactionCloseDate = tx.CloseDate;
                dailyData.TransactionClosingDescription = tx.ClosingDescription;
                tx.DailyDataCollection.Add(dailyData);
            }
        });
    }

    public async Task<ObservableCollection<Transaction>> GetLastXTransactions(int count)
    {
        var txs = (await facade.TransactionManager.GetAllTransactions())
            .OrderByDescending(q => q.EntryDate).Take(count).ToList();
        return await LoadDetails(txs);
    }

    public async Task<ObservableCollection<Transaction>> GetAllOpenTransactions()
    {
        var txs = (await facade.TransactionManager.GetAllTransactions())
            .Where(q => !q.IsClosed).OrderByDescending(q => q.EntryDate).ToList();
        return await LoadDetails(txs);
    }

    public async Task<ObservableCollection<Transaction>> GetTransactionsForCompany(int companyId)
    {
        var txs = (await facade.TransactionManager.GetTransactionsForCompany(companyId))
            .OrderByDescending(q => q.EntryDate).ToList();
        return await LoadDetails(txs);
    }

    private async Task<ObservableCollection<Transaction>> LoadDetails(List<Transaction> transactions)
    {
        foreach (var tx in transactions)
        {
            var dailyData = (await facade.DailyDataProvider.GetDailyDataForTransactionAsync(tx.ID))
                .OrderBy(q => q.Date).ToList();

            var allEvents = await facade.EventManager.GetEventsForCompany(tx.CompanyID);

            foreach (var item in dailyData)
            {
                var match = allEvents.FirstOrDefault(q => q.EntryDate.Date == item.Date.Date);
                item.EventDate = match?.EntryDate.Date;
                item.TransactionCloseDate = tx.CloseDate;
                item.TransactionClosingDescription = tx.ClosingDescription;
            }

            tx.DailyDataCollection = new ObservableCollection<DailyData>(dailyData);
            tx.Comments = new ObservableCollection<Comment>(
                await facade.CommentManager.GetCommentsForTransaction(tx.ID));
        }

        return new ObservableCollection<Transaction>(transactions);
    }
}