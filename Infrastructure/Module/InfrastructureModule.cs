using DataAccess.Data;
using DataAccess.DBAccess;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using MahApps.Metro.Controls.Dialogs;

namespace Infrastructure.Module;

public class InfrastructureModule : IModule
{
    public void OnInitialized(IContainerProvider containerProvider) { }

    public void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterSingleton<IDailyTracker, DailyTracker>();
        containerRegistry.RegisterSingleton<ITransactionData, TransactionData>();
        containerRegistry.RegisterSingleton<ICommentData, CommentData>();
        containerRegistry.RegisterSingleton<IEventData, EventData>();
        containerRegistry.RegisterSingleton<ICompanyData, CompanyData>();
        containerRegistry.RegisterSingleton<ISQLDataAccess, SQLDataAccess>();
        containerRegistry.RegisterSingleton<IDialogCoordinator, DialogCoordinator>();
        containerRegistry.RegisterSingleton<IDailyDataProvider, DailyDataProvider>();
        containerRegistry.RegisterSingleton<ICommentManager, CommentManager>();
        containerRegistry.RegisterSingleton<IViewManager, ViewManager>();
        containerRegistry.RegisterSingleton<ITransactionManager, TransactionManager>();
        containerRegistry.RegisterSingleton<IEventManager, EventManager>();
        containerRegistry.RegisterSingleton<ICompanyManager, CompanyManager>();
        containerRegistry.RegisterSingleton<ITradeTrackerFacade, TradeTrackerFacade>();
        containerRegistry.RegisterSingleton<ViewManager>();
    }
}
