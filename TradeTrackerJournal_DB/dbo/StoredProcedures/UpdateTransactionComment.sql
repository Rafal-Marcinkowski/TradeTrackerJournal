CREATE PROCEDURE UpdateTransactionComment
    @ID INT,
    @EntryDate DATETIME,
    @CommentText NVARCHAR(250)
AS
BEGIN
    UPDATE [dbo].[TransactionComments]
    SET EntryDate = @EntryDate, CommentText = @CommentText
    WHERE ID = @ID;
END;
