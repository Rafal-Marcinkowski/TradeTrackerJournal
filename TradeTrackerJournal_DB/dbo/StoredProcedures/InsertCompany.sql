CREATE PROCEDURE InsertCompany
    @CompanyName NVARCHAR(35),
    @TransactionCount INT = NULL,
    @EventCount INT = NULL
AS
BEGIN
    INSERT INTO [dbo].[Companies] (CompanyName, TransactionCount, EventCount)
    VALUES (@CompanyName, @TransactionCount, @EventCount);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
