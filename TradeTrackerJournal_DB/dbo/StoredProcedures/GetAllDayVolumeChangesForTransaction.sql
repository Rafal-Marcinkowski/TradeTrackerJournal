CREATE PROCEDURE GetAllDayVolumeChangesForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT [ID], TransactionID, DayIndex, DayVolumeChange
    FROM [dbo].[TransactionDayVolumeChanges]
    WHERE TransactionID = @TransactionID;
END;
