CREATE PROCEDURE InsertCompany
    @CompanyName NVARCHAR(30),
    @TransactionCount INT = NULL
AS
BEGIN
    INSERT INTO [dbo].[Companies] (CompanyName, TransactionCount)
    VALUES (@CompanyName, @TransactionCount);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
