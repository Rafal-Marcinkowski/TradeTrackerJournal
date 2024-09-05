using SharedProject.Models;

namespace DataAccess.Data;
public interface ICommentData
{
    Task DeleteCommentAsync(int id);
    Task<IEnumerable<Comment>> GetAllCommentsAsync();
    Task<IEnumerable<Comment>> GetAllCommentsForEventAsync(int eventId);
    Task<IEnumerable<Comment>> GetAllCommentsForTransactionAsync(int transactionId);
    Task<Comment> GetCommentAsync(int id);
    Task<int> GetCommentID(string commentText);
    Task InsertCommentAsync(Comment comment);
    Task UpdateCommentAsync(Comment comment);
}