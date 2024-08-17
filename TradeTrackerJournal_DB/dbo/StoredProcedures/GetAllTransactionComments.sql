CREATE PROCEDURE GetAllTransactionComments
AS
BEGIN
    SELECT * FROM [dbo].[TransactionComments];
END;
