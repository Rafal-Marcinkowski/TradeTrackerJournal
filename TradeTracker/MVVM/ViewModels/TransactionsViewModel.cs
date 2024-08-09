using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsViewModel : BindableBase
{
    private readonly IRegionManager regionManager;

    public TransactionsViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
    }

    public ICommand NavigateToAddTransactionCommand => new DelegateCommand(() =>
    {
        regionManager.RequestNavigate("MainRegion", nameof(AddTransactionView));
    });

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        regionManager.RequestNavigate("MainRegion", nameof(OpenPositionsView));
    });

    public ICommand NavigateToTransactionsOverviewCommand => new DelegateCommand(() =>
    {
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView));
    });
}
