using DataAccess.DBAccess;
using SharedModels.Models;

namespace DataAccess.Data;

public class TransactionData : ITransactionData
{
    private readonly ISQLDataAccess dBAccess;

    public TransactionData(ISQLDataAccess dBAccess)
    {
        this.dBAccess = dBAccess;
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsAsync()
    {
        return await dBAccess.LoadDataAsync<Transaction, dynamic>("GetAllTransactions", new { });
    }

    public async Task<Transaction> GetTransactionAsync(int id)
    {
        var transactions = await dBAccess.LoadDataAsync<Transaction, dynamic>("GetTransaction", new { ID = id });
        return transactions.FirstOrDefault();
    }

    public async Task InsertTransactionAsync(Transaction transaction)
    {
        var parameters = new
        {
            transaction.CompanyID,
            transaction.CompanyName,
            transaction.EntryDate,
            transaction.EntryPrice,
            transaction.EntryMedianVolume,
            transaction.NumberOfShares,
            transaction.PositionSize,
            transaction.CloseDate,
            transaction.AvgSellPrice,
            transaction.IsClosed,
            transaction.Duration,
            transaction.InitialDescription,
            transaction.ClosingDescription,
            transaction.InformationLink
        };

        await dBAccess.SaveDataAsync("InsertTransaction", parameters);
    }

    public async Task UpdateTransactionAsync(
      int id,
      int companyID,
      string companyName,
      DateTime entryDate,
      decimal? entryPrice,
      int entryMedianVolume,
      int numberOfShares,
      int positionSize,
      DateTime? closeDate,
      decimal? avgSellPrice,
      bool isClosed,
      int duration,
      string initialDescription,
      string closingDescription,
      string informationLink)
    {
        var parameters = new
        {
            ID = id,
            CompanyID = companyID,
            CompanyName = companyName,
            EntryDate = entryDate,
            EntryPrice = entryPrice,
            EntryMedianVolume = entryMedianVolume,
            NumberOfShares = numberOfShares,
            PositionSize = positionSize,
            CloseDate = closeDate,
            AvgSellPrice = avgSellPrice,
            IsClosed = isClosed,
            Duration = duration,
            InitialDescription = initialDescription,
            ClosingDescription = closingDescription,
            InformationLink = informationLink
        };

        await dBAccess.SaveDataAsync("UpdateTransaction", parameters);
    }

    public async Task DeleteTransactionAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteTransaction", new { ID = id });
    }
}
