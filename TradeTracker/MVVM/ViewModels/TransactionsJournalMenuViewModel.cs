using Infrastructure.Services;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsJournalMenuViewModel(ViewManager viewManager) : BindableBase
{
    public ICommand NavigateToAddTransactionCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(AddTransactionView)));

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(OpenPositionsView)));

    public ICommand NavigateToTransactionsOverviewMenuCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(TransactionsOverviewMenuView)));
}
