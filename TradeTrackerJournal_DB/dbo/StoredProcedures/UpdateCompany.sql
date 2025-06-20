CREATE PROCEDURE UpdateCompany
    @ID INT,
    @CompanyName NVARCHAR(35),
    @TransactionCount INT = NULL,
    @EventCount INT = NULL,
    @NoteCount INT = NULL
AS
BEGIN
    UPDATE [dbo].[Companies]
    SET CompanyName = @CompanyName, TransactionCount = @TransactionCount, EventCount = @EventCount, NoteCount = @NoteCount
    WHERE ID = @ID;
END;
