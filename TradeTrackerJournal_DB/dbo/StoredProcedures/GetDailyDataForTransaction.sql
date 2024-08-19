CREATE PROCEDURE GetDailyDataForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT * FROM DailyData WHERE TransactionID = @TransactionID;
END
