using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Windows.Input;
using TradeTracker.MVVM.Views;

namespace TradeTracker.MVVM.ViewModels;

public class TransactionsJournalMenuViewModel : BindableBase
{
    private readonly IRegionManager regionManager;

    public TransactionsJournalMenuViewModel(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
    }

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
