using Prism.Commands;
using Prism.Regions;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

class TransactionsOverviewMenuViewModel
{
    private readonly IRegionManager regionManager;

    public TransactionsOverviewMenuViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
    }

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewView));
    });
}
