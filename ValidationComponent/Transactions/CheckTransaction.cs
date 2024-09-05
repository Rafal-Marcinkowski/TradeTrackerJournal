using DataAccess.Data;
using SharedProject.Models;

namespace ValidationComponent.Transactions;

public class CheckTransaction(ITransactionData transactionData)
{
    public async Task<bool> IsExisting(Transaction transaction)
    {
        var transactions = await transactionData.GetAllTransactionsForCompany(transaction.CompanyID);
        var comparer = new TransactionComparer();
        return transactions.Any(q => comparer.Equals(transaction, q));
    }
}
