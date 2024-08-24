CREATE PROCEDURE UpdateDailyData
    @ID INT,
    @TransactionID INT,
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
    UPDATE DailyData
    SET TransactionID = @TransactionID,
        Date = @Date,
        OpenPrice = @OpenPrice,
        ClosePrice = @ClosePrice,
        Volume = @Volume,
        Turnover = @Turnover,
        MinPrice = @MinPrice,
        MaxPrice = @MaxPrice,
        PriceChange = @PriceChange,
        TurnoverChange = @TurnoverChange,
        TransactionCount =@TransactionCount
    WHERE ID = @ID;
END
