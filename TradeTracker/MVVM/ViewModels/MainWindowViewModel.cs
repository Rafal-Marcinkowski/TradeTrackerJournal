using EventTracker.MVVM.Views;
using HotStockTracker.MVVM.Views;
using Infrastructure.Services;
using SessionOpening;
using StockNotepad.MVVM.Views;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class MainWindowViewModel(ViewManager viewManager) : BindableBase
{
    public ICommand NavigateToEventsCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(EventsMainMenuView)));

    public ICommand NavigateToTransactionsCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(TransactionsJournalMenuView)));

    public ICommand NavigateToSessionOpeningCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(SessionOpeningView)));

    public ICommand NavigateToHotStockCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(HotStockOverviewView)));

    public ICommand NavigateToStockNotepadCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(StockNotepadSelectCompanyView)));

    public ICommand NavigateToCompanyRenameCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(CompanyRenameView)));

    public ICommand NavigateToAddCompanyCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(AddCompanyView)));

    public ICommand NavigateToDeleteCompanyCommand => new DelegateCommand(() => viewManager.NavigateTo(nameof(DeleteCompanyView)));

    public ICommand NavigateToLast10TransactionsCommand => new DelegateCommand(() =>
    {
        var parameters = new NavigationParameters()
                {
                    {"lastx", 10 },
                };
        viewManager.NavigateTo(nameof(TransactionsOverviewView), parameters);
    });

    public ICommand NavigateToLast10EventsCommand => new DelegateCommand(() =>
    {
        var parameters = new NavigationParameters()
                {
                    {"lastx", 10 },
                };
        viewManager.NavigateTo(nameof(EventsOverviewView), parameters);
    });
}
