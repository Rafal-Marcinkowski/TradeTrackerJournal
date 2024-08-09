using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class MainWindowViewModel : BindableBase
{
    private readonly IRegionManager regionManager;

    public MainWindowViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
    }

    public ICommand NavigateToEventsCommand => new DelegateCommand(() =>
    {
        regionManager.RequestNavigate("MainRegion", nameof(EventsView));
    });

    public ICommand NavigateToTransactionsCommand => new DelegateCommand(() =>
    {
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsView));
    });
}
