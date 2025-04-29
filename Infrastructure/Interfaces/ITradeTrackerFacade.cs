using DataAccess.Data;
using Infrastructure.Services;

namespace Infrastructure.Interfaces;

public interface ITradeTrackerFacade
{
    ICommentManager CommentManager { get; }
    ICompanyManager CompanyManager { get; }
    IEventAggregator EventAggregator { get; }
    IEventManager EventManager { get; }
    ITransactionManager TransactionManager { get; }
    IDailyDataProvider DailyDataProvider { get; }
    ViewManager ViewManager { get; }
}