CREATE PROCEDURE UpdateComment
    @ID INT,
    @EntryDate DATETIME,
    @CommentText NVARCHAR(250)
AS
BEGIN
    UPDATE [dbo].[Comments]
    SET EntryDate = @EntryDate, CommentText = @CommentText
    WHERE ID = @ID;
END;
