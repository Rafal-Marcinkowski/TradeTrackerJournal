using SharedProject.Models;

namespace Infrastructure.Interfaces;
public interface ICommentManager
{
    Task AddComment(Comment comment);
    Task DeleteComment(int commentId);
    Task<IEnumerable<Comment>> GetCommentsForTransaction(int transactionId);
    Task<IEnumerable<Comment>> GetCommentsForEvent(int eventId);
    Task UpdateComment(Comment comment);
}