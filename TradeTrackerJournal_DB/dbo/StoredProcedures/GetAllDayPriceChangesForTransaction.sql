CREATE PROCEDURE GetAllDayPriceChangesForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT [ID], TransactionID, DayIndex, DayPriceChange
    FROM [dbo].[TransactionDayPriceChanges]
    WHERE TransactionID = @TransactionID;
END;
