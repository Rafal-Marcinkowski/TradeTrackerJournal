CREATE PROCEDURE GetAllTransactionsForCompany
    @CompanyID INT
AS
BEGIN
    SELECT *
    FROM [dbo].[Transactions]
    WHERE CompanyID = @CompanyID;
END;
