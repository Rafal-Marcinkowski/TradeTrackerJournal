using DataAccess.Data;
using Infrastructure.Interfaces;
using SharedProject.Models;

namespace Infrastructure.Services;

public class CommentManager(ICommentData commentData) : ICommentManager
{
    public async Task AddComment(Comment comment)
    {
        await commentData.InsertCommentAsync(comment);
    }

    public async Task UpdateComment(Comment comment)
    {
        await commentData.UpdateCommentAsync(comment);
    }

    public async Task DeleteComment(int commentId)
    {
        await commentData.DeleteCommentAsync(commentId);
    }

    public async Task<IEnumerable<Comment>> GetCommentsForTransaction(int transactionId)
    {
        return await commentData.GetAllCommentsForTransactionAsync(transactionId);
    }

    public async Task<IEnumerable<Comment>> GetCommentsForEvent(int eventId)
    {
        return await commentData.GetAllCommentsForEventAsync(eventId);
    }
}
