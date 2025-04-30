using SharedProject.Models;
using SharedProject.ViewModels;

namespace Infrastructure.Interfaces;

public interface ITransactionManager
{
    Task AddTransaction(Transaction Transaction);
    Task CloseTransaction(Transaction transaction);
    Task<IEnumerable<Transaction>> GetAllTransactions();
    Task<IEnumerable<Transaction>> GetLastXTransactions(int count);
    Task<IEnumerable<Transaction>> GetOpenTransactions();
    Task<Transaction> GetTransaction(int transactionId);
    Task<IEnumerable<Transaction>> GetTransactionsForCompany(int companyId);
    Task<IEnumerable<Transaction>> LoadAndSetOpenTransactions();
    Task<bool> TryAddTransaction(TransactionEventViewModel transactionVM);
    Task UpdateTransaction(Transaction Transaction);
}