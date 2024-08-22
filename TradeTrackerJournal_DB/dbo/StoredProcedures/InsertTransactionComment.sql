CREATE PROCEDURE InsertTransactionComment
    @TransactionID INT,
    @EntryDate DATETIME,
    @CommentText NVARCHAR(250)
AS
BEGIN
    INSERT INTO [dbo].[TransactionComments] (TransactionID, EntryDate, CommentText)
    VALUES (@TransactionID, @EntryDate, @CommentText);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
