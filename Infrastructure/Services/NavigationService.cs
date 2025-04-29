using Infrastructure.Interfaces;
using SharedProject.Models;
using System.Collections.ObjectModel;

namespace Infrastructure.Services;

public class NavigationService(ITradeTrackerFacade facade)
{
    public async Task<ObservableCollection<Transaction>> LoadTransactionsBasedOnContext(NavigationContext context)
    {
        var loader = new TransactionLoader(facade);

        foreach (var key in context.Parameters.Keys)
        {
            switch (key)
            {
                case "selectedCompany": return await loader.GetTransactionsForCompany((int)context.Parameters[key]);
                case "op": return await loader.GetAllOpenTransactions();
                case "lastx": return await loader.GetLastXTransactions((int)context.Parameters[key]);
            }
        }

        return [];
    }
}
