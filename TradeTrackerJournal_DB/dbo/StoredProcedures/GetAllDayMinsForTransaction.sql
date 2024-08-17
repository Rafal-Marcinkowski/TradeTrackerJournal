CREATE PROCEDURE GetAllDayMinsForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT [ID], TransactionID, DayIndex, DayMin
    FROM [dbo].[TransactionDayMins]
    WHERE TransactionID = @TransactionID;
END;
