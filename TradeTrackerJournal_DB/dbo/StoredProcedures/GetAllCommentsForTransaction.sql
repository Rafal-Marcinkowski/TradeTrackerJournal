CREATE PROCEDURE GetAllCommentsForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT *
    FROM [dbo].[TransactionComments]
    WHERE TransactionID = @TransactionID;
END;
