CREATE PROCEDURE UpdateTransactionComment
    @ID INT,
    @CommentText NVARCHAR(250)
AS
BEGIN
    UPDATE [dbo].[TransactionComments]
    SET CommentText = @CommentText
    WHERE ID = @ID;
END;
