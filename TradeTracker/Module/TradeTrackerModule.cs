using TradeTracker.MVVM.ViewModels;
using TradeTracker.MVVM.Views;

namespace TradeTracker.Module;

public class TradeTrackerModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.Register<AddCompanyViewModel>();
        containerRegistry.Register<DeleteCompanyViewModel>();
        containerRegistry.Register<MainWindowViewModel>();
        containerRegistry.Register<TransactionsJournalMenuViewModel>();
        containerRegistry.Register<AddTransactionViewModel>();
        containerRegistry.Register<OpenPositionsViewModel>();
        containerRegistry.Register<CompanyRenameViewModel>();
        containerRegistry.Register<TransactionsOverviewViewModel>();

        containerRegistry.RegisterForNavigation<DeleteCompanyView>();
        containerRegistry.RegisterForNavigation<AddCompanyView>();
        containerRegistry.RegisterForNavigation<AddTransactionView>();
        containerRegistry.RegisterForNavigation<MainWindow>();
        containerRegistry.RegisterForNavigation<OpenPositionsView>();
        containerRegistry.RegisterForNavigation<TransactionsJournalMenuView>();
        containerRegistry.RegisterForNavigation<CompanyRenameView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewMenuView>();
        containerRegistry.RegisterForNavigation<TransactionsOverviewView>();
    }
}
