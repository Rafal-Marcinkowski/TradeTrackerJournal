CREATE PROCEDURE GetAllDayOpenPricesForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT [ID], TransactionID, DayIndex, DayOpenPrice
    FROM [dbo].[TransactionDayOpenPrices]
    WHERE TransactionID = @TransactionID;
END;
