using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using SessionOpening;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class MainWindowViewModel(IRegionManager regionManager) : BindableBase
{
    public ICommand NavigateToEventsCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(EventsView));
    });

    public ICommand NavigateToTransactionsCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsJournalMenuView));
    });

    public ICommand NavigateToSessionOpeningCommand => new DelegateCommand(() =>
    {
        regionManager.RequestNavigate("MainRegion", nameof(SessionOpeningView));
    });
}
