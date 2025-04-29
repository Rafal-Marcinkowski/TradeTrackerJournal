using EventTracker.MVVM.Views;
using Infrastructure.Services;
using SessionOpening;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class MainWindowViewModel(ViewManager viewManager) : BindableBase
{
    public ICommand NavigateToEventsCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(EventsMainMenuView)));

    public ICommand NavigateToTransactionsCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(TransactionsJournalMenuView)));

    public ICommand NavigateToSessionOpeningCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(SessionOpeningView)));
}
