using DataAccess.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Services;

public class TradeTrackerFacade(
    ITransactionManager transactionManager,
    IEventManager eventManager,
    ICompanyManager companyManager,
    ICommentManager commentManager,
    IEventAggregator eventAggregator,
    IDailyDataProvider dailyDataProvider,
    IViewManager viewManager) : ITradeTrackerFacade
{
    public ITransactionManager TransactionManager { get; } = transactionManager;
    public IEventManager EventManager { get; } = eventManager;
    public ICompanyManager CompanyManager { get; } = companyManager;
    public ICommentManager CommentManager { get; } = commentManager;
    public IEventAggregator EventAggregator { get; } = eventAggregator;
    public IDailyDataProvider DailyDataProvider { get; } = dailyDataProvider;
    public IViewManager ViewManager { get; } = viewManager;
}
