CREATE PROCEDURE InsertCompany
    @CompanyName NVARCHAR(35),
    @TransactionCount INT = NULL,
    @EventCount INT = NULL,
    @NoteCount INT = NULL
AS
BEGIN
    INSERT INTO [dbo].[Companies] (CompanyName, TransactionCount, EventCount, NoteCount)
    VALUES (@CompanyName, @TransactionCount, @EventCount, @NoteCount);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
