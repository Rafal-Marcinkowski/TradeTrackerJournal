CREATE PROCEDURE UpdateDailyData
    @ID INT,
    @TransactionID INT,
    @Date DATETIME,
    @OpenPrice DECIMAL(18,2),
    @ClosePrice DECIMAL(18,2),
    @Volume DECIMAL(18,2),
    @MinPrice DECIMAL(18,2),
    @MaxPrice DECIMAL(18,2),
    @PriceChange DECIMAL(18,2),
    @VolumeChange DECIMAL(18,2)
AS
BEGIN
    UPDATE DailyData
    SET TransactionID = @TransactionID,
        Date = @Date,
        OpenPrice = @OpenPrice,
        ClosePrice = @ClosePrice,
        Volume = @Volume,
        MinPrice = @MinPrice,
        MaxPrice = @MaxPrice,
        PriceChange = @PriceChange,
        VolumeChange = @VolumeChange
    WHERE ID = @ID;
END
