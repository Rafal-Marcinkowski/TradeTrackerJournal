using Infrastructure.Events;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsOverviewViewModel : BindableBase, INavigationAware
{
    private readonly ITradeTrackerFacade _facade;
    private readonly TransactionLoader _transactionLoader;

    public ObservableCollection<Transaction> Transactions { get; } = [];
    public CommandManager Commands { get; }

    public TransactionsOverviewViewModel(ITradeTrackerFacade facade)
    {
        _facade = facade;
        _transactionLoader = new TransactionLoader(facade);
        Commands = new CommandManager(facade);

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        _facade.EventAggregator.GetEvent<DailyDataAddedEvent>()
            .Subscribe(d => _transactionLoader.OnDailyDataAdded(d, Transactions));

        _facade.EventAggregator.GetEvent<TransactionUpdatedEvent>()
            .Subscribe(t => _transactionLoader.OnTransactionUpdated(t, Transactions));
    }

    public async void OnNavigatedTo(NavigationContext navigationContext)
    {
        foreach (var key in navigationContext.Parameters.Keys)
        {
            await (key switch
            {
                "selectedCompany" => _transactionLoader.GetTransactionsForCompany((int)navigationContext.Parameters[key]),
                "op" => _transactionLoader.GetAllOpenTransactions(),
                "lastx" => _transactionLoader.GetLastXTransactions((int)navigationContext.Parameters[key]),
                _ => Task.FromResult(new ObservableCollection<Transaction>())
            }).ContinueWith(t =>
            {
                Transactions.Clear();
                foreach (var item in t.Result)
                    Transactions.Add(item);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    public void OnNavigatedFrom(NavigationContext navigationContext) { }
}