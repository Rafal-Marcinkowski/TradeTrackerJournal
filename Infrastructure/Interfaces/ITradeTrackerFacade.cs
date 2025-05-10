using DataAccess.Data;

namespace Infrastructure.Interfaces;

public interface ITradeTrackerFacade
{
    ICommentManager CommentManager { get; }
    ICompanyManager CompanyManager { get; }
    IDailyDataProvider DailyDataProvider { get; }
    IEventAggregator EventAggregator { get; }
    IEventManager EventManager { get; }
    ITransactionManager TransactionManager { get; }
    IViewManager ViewManager { get; }
}