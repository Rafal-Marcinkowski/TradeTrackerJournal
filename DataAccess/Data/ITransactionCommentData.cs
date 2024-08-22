using SharedModels.Models;

namespace DataAccess.Data;
public interface ITransactionCommentData
{
    Task DeleteCommentAsync(int id);
    Task<IEnumerable<TransactionComment>> GetAllCommentsAsync();
    Task<IEnumerable<TransactionComment>> GetAllCommentsForTransactionAsync(int transactionId);
    Task<TransactionComment> GetCommentAsync(int id);
    Task<int> GetCommentID(string commentText);
    Task InsertCommentAsync(TransactionComment comment);
    Task UpdateCommentAsync(TransactionComment comment);
}