CREATE PROCEDURE GetAllDayVolumesForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT [ID], TransactionID, DayIndex, DayVolume
    FROM [dbo].[TransactionDayVolumes]
    WHERE TransactionID = @TransactionID;
END;
