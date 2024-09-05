using DataAccess.DBAccess;
using SharedProject.Models;

namespace DataAccess.Data;

public class TransactionData(ISQLDataAccess dBAccess) : ITransactionData
{
    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await dBAccess.LoadDataAsync<Transaction, dynamic>("GetAllTransactions", new { });
    }

    public async Task<Transaction> GetTransactionAsync(int id)
    {
        var transactions = await dBAccess.LoadDataAsync<Transaction, dynamic>("GetTransaction", new { ID = id });
        return transactions.FirstOrDefault();
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsForCompany(int companyId)
    {
        var parameters = new { CompanyID = companyId };
        var transactions = await dBAccess.LoadDataAsync<Transaction, dynamic>("GetAllTransactionsForCompany", parameters);
        return transactions;
    }

    public async Task InsertTransactionAsync(Transaction transaction)
    {
        var parameters = new
        {
            transaction.CompanyID,
            transaction.CompanyName,
            transaction.EntryDate,
            transaction.EntryPrice,
            transaction.EntryMedianTurnover,
            transaction.NumberOfShares,
            transaction.PositionSize,
            transaction.CloseDate,
            transaction.AvgSellPrice,
            transaction.IsClosed,
            transaction.InitialDescription,
            transaction.ClosingDescription,
            transaction.InformationLink,
            transaction.IsTracking
        };

        await dBAccess.SaveDataAsync("InsertTransaction", parameters);
    }

    public async Task UpdateTransactionAsync(Transaction transaction)
    {
        var parameters = new
        {
            transaction.ID,
            transaction.CompanyID,
            transaction.CompanyName,
            transaction.EntryDate,
            transaction.EntryPrice,
            transaction.EntryMedianTurnover,
            transaction.NumberOfShares,
            transaction.PositionSize,
            transaction.CloseDate,
            transaction.AvgSellPrice,
            transaction.IsClosed,
            transaction.InitialDescription,
            transaction.ClosingDescription,
            transaction.InformationLink,
            transaction.IsTracking
        };

        await dBAccess.SaveDataAsync("UpdateTransaction", parameters);
    }

    public async Task<int> GetID(Transaction transaction)
    {
        var transactions = await GetAllTransactionsForCompany(transaction.CompanyID);
        return transactions.FirstOrDefault(q => q.CompanyID == transaction.CompanyID && q.EntryDate == transaction.EntryDate
                                                 && q.EntryPrice == transaction.EntryPrice && q.PositionSize == transaction.PositionSize && q.NumberOfShares == transaction.NumberOfShares).ID;
    }

    public async Task DeleteTransactionAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteTransaction", new { ID = id });
    }
}
