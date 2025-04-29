using SharedProject.Models;

namespace Infrastructure.Interfaces;
public interface ICommentManager
{
    Task AddComment(Comment comment);
    Task DeleteComment(int commentId);
    Task<IEnumerable<Comment>> GetCommentsForTransaction(int transactionId);
    Task UpdateComment(Comment comment);
}