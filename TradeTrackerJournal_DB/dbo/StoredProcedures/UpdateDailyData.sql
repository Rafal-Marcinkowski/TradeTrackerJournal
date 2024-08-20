CREATE PROCEDURE UpdateDailyData
    @ID INT,
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
