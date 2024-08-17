CREATE PROCEDURE GetAllEndOfDayPricesForTransaction
    @TransactionID INT
AS
BEGIN
    SELECT [ID], TransactionID, DayIndex, EndOfDayPrice
    FROM [dbo].[TransactionEndOfDayPrices]
    WHERE TransactionID = @TransactionID;
END;
