CREATE PROCEDURE InsertComment
    @TransactionID INT,
    @EventID INT,
    @EntryDate DATETIME,
    @CommentText NVARCHAR(250)
AS
BEGIN
    INSERT INTO [dbo].[Comments] (TransactionID, EventID,  EntryDate, CommentText)
    VALUES (@TransactionID, @EventID,  @EntryDate, @CommentText);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
