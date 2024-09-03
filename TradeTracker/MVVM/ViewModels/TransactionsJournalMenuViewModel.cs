using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsJournalMenuViewModel(IRegionManager regionManager) : BindableBase
{
    public ICommand NavigateToAddTransactionCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(AddTransactionView));
    });

    public ICommand NavigateToOpenPositionsCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(OpenPositionsView));
    });

    public ICommand NavigateToTransactionsOverviewMenuCommand => new DelegateCommand(() =>
    {
        var region = regionManager.Regions["MainRegion"];
        region.RemoveAll();
        regionManager.RequestNavigate("MainRegion", nameof(TransactionsOverviewMenuView));
    });
}
