CREATE PROCEDURE InsertDailyData
    @TransactionID INT,
    @EventID INT,
    @Date DATETIME,
    @OpenPrice DECIMAL(12,2),
    @ClosePrice DECIMAL(12,2),
    @Volume DECIMAL(12,2),
    @Turnover DECIMAL(12,2),
    @MinPrice DECIMAL(12,2),
    @MaxPrice DECIMAL(12,2),
    @PriceChange DECIMAL(12,2),
    @TurnoverChange DECIMAL(12,2),
    @TransactionCount INT
AS
BEGIN
    INSERT INTO DailyData (TransactionID, EventID,  Date, OpenPrice, ClosePrice, Volume, Turnover, MinPrice, MaxPrice, PriceChange, TurnoverChange, TransactionCount)
    VALUES (@TransactionID, @EventID, @Date, @OpenPrice, @ClosePrice, @Volume, @Turnover, @MinPrice, @MaxPrice, @PriceChange, @TurnoverChange, @TransactionCount);
END
