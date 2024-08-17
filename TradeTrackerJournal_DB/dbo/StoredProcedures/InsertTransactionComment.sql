CREATE PROCEDURE InsertTransactionComment
    @TransactionID INT,
    @CommentText NVARCHAR(250)
AS
BEGIN
    INSERT INTO [dbo].[TransactionComments] (TransactionID, CommentText)
    VALUES (@TransactionID, @CommentText);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
