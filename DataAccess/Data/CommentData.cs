using DataAccess.DBAccess;
using SharedProject.Models;

namespace DataAccess.Data;

public class CommentData(ISQLDataAccess dBAccess) : ICommentData
{
    public async Task<IEnumerable<Comment>> GetAllCommentsAsync()
    {
        return await dBAccess.LoadDataAsync<Comment, dynamic>("GetAllComments", new { });
    }

    public async Task<Comment> GetCommentAsync(int id)
    {
        var comments = await dBAccess.LoadDataAsync<Comment, dynamic>("GetComment", new { ID = id });
        return comments.FirstOrDefault();
    }

    public async Task<IEnumerable<Comment>> GetAllCommentsForTransactionAsync(int transactionId)
    {
        var parameters = new { TransactionID = transactionId };
        var comments = await dBAccess.LoadDataAsync<Comment, dynamic>("GetAllCommentsForTransaction", parameters);
        return comments;
    }

    public async Task<IEnumerable<Comment>> GetAllCommentsForEventAsync(int eventId)
    {
        var parameters = new { EventID = eventId };
        var comments = await dBAccess.LoadDataAsync<Comment, dynamic>("GetAllCommentsForEvent", parameters);
        return comments;
    }

    public async Task InsertCommentAsync(Comment comment)
    {
        var parameters = new
        {
            comment.TransactionID,
            comment.EventID,
            comment.EntryDate,
            comment.CommentText
        };
        await dBAccess.SaveDataAsync("InsertComment", parameters);
    }

    public async Task UpdateCommentAsync(Comment comment)
    {
        var parameters = new
        {
            comment.ID,
            comment.EntryDate,
            comment.CommentText
        };
        await dBAccess.SaveDataAsync("UpdateComment", parameters);
    }

    public async Task DeleteCommentAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteComment", new { ID = id });
    }

    public async Task<int> GetCommentID(string commentText)
    {
        var comments = await GetAllCommentsAsync();
        return comments.FirstOrDefault(q => q.CommentText == commentText)?.ID ?? -1;
    }
}
