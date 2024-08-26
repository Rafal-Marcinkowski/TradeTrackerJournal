using DataAccess.DBAccess;
using SharedModels.Models;

namespace DataAccess.Data;

public class TransactionCommentData : ITransactionCommentData
{
    private readonly ISQLDataAccess dBAccess;

    public TransactionCommentData(ISQLDataAccess dBAccess)
    {
        this.dBAccess = dBAccess;
    }

    public async Task<IEnumerable<TransactionComment>> GetAllCommentsAsync()
    {
        return await dBAccess.LoadDataAsync<TransactionComment, dynamic>("GetAllTransactionComments", new { });
    }

    public async Task<TransactionComment> GetCommentAsync(int id)
    {
        var comments = await dBAccess.LoadDataAsync<TransactionComment, dynamic>("GetTransactionComment", new { ID = id });
        return comments.FirstOrDefault();
    }

    public async Task<IEnumerable<TransactionComment>> GetAllCommentsForTransactionAsync(int transactionId)
    {
        var parameters = new { TransactionID = transactionId };
        var comments = await dBAccess.LoadDataAsync<TransactionComment, dynamic>("GetAllCommentsForTransaction", parameters);
        return comments;
    }

    public async Task InsertCommentAsync(TransactionComment comment)
    {
        var parameters = new
        {
            comment.TransactionID,
            comment.EntryDate,
            comment.CommentText
        };
        await dBAccess.SaveDataAsync("InsertTransactionComment", parameters);
    }

    public async Task UpdateCommentAsync(TransactionComment comment)
    {
        var parameters = new
        {
            comment.ID,
            comment.EntryDate,
            comment.CommentText
        };
        await dBAccess.SaveDataAsync("UpdateTransactionComment", parameters);
    }

    public async Task DeleteCommentAsync(int id)
    {
        await dBAccess.SaveDataAsync("DeleteTransactionComment", new { ID = id });
    }

    public async Task<int> GetCommentID(string commentText)
    {
        var comments = await GetAllCommentsAsync();
        return comments.FirstOrDefault(q => q.CommentText == commentText)?.ID ?? -1;
    }
}
