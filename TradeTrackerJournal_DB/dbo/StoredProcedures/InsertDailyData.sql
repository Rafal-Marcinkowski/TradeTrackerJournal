CREATE PROCEDURE InsertDailyData
    @TransactionID INT,
    @Date DATETIME,
    @OpenPrice DECIMAL(8,2),
    @ClosePrice DECIMAL(8,2),
    @Volume DECIMAL(8,2),
    @Turnover DECIMAL(8,2),
    @MinPrice DECIMAL(8,2),
    @MaxPrice DECIMAL(8,2),
    @PriceChange DECIMAL(8,2),
    @TurnoverChange DECIMAL(8,2),
    @TransactionCount INT
AS
BEGIN
    INSERT INTO DailyData (TransactionID, Date, OpenPrice, ClosePrice, Volume, Turnover, MinPrice, MaxPrice, PriceChange, TurnoverChange, TransactionCount)
    VALUES (@TransactionID, @Date, @OpenPrice, @ClosePrice, @Volume, @Turnover, @MinPrice, @MaxPrice, @PriceChange, @TurnoverChange, @TransactionCount);
END
