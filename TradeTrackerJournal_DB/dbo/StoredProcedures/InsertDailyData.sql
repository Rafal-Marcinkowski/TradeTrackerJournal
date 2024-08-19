CREATE PROCEDURE InsertDailyData
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
    INSERT INTO DailyData (TransactionID, Date, OpenPrice, ClosePrice, Volume, MinPrice, MaxPrice, PriceChange, VolumeChange)
    VALUES (@TransactionID, @Date, @OpenPrice, @ClosePrice, @Volume, @MinPrice, @MaxPrice, @PriceChange, @VolumeChange);
END
