CREATE PROCEDURE GetAllCommentsForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT *
    FROM [dbo].[Comments]
    WHERE TransactionID = @TransactionID;
END;
