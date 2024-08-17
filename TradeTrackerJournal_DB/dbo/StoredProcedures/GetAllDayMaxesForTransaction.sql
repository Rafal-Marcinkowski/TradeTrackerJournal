CREATE PROCEDURE GetAllDayMaxesForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT [ID], TransactionID, DayIndex, DayMaxes
    FROM [dbo].[TransactionDayMaxes]
    WHERE TransactionID = @TransactionID;
END;
