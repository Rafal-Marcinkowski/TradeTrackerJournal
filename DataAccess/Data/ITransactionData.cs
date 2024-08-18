using SharedModels.Models;

namespace DataAccess.Data;
public interface ITransactionData
{
    Task DeleteTransactionAsync(int id);
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<Transaction> GetTransactionAsync(int id);
    Task InsertTransactionAsync(Transaction transaction);
    Task UpdateTransactionAsync(int id, int companyID, string companyName, DateTime entryDate, decimal? entryPrice, int entryMedianVolume, int numberOfShares, int positionSize, DateTime? closeDate, decimal? avgSellPrice, bool isClosed, int duration, string initialDescription, string closingDescription, string informationLink);
}