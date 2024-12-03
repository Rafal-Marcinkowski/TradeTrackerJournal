using DataAccess.Data;

namespace Infrastructure;

public class DependencyManager(IRegionManager regionManager, ICommentData commentData, ICompanyData companyData, ITransactionData transactionData, IEventAggregator eventAggregator
    , IEventData eventData, IDailyDataProvider dailyDataProvider)
{

}
