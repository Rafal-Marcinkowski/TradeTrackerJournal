CREATE PROCEDURE UpdateCompany
    @ID INT,
    @CompanyName NVARCHAR(30),
    @TransactionCount INT = NULL
AS
BEGIN
    UPDATE [dbo].[Companies]
    SET CompanyName = @CompanyName, TransactionCount = @TransactionCount
    WHERE ID = @ID;
END;
