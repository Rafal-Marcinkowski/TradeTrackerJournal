using SharedProject.Models;

namespace DataAccess.Data;
public interface ITransactionData
{
    Task DeleteTransactionAsync(int id);
    Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
    Task<IEnumerable<Transaction>> GetAllTransactionsForCompany(int companyId);
    Task<int> GetID(Transaction transaction);
    Task<Transaction> GetTransactionAsync(int id);
    Task InsertTransactionAsync(Transaction transaction);
    Task UpdateTransactionAsync(Transaction transaction);
}