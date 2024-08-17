CREATE PROCEDURE UpdateTransaction
    @ID INT,
    @CompanyID INT,
    @CompanyName NVARCHAR(30),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(6, 2) = NULL,
    @EntryMedianVolume INT,
    @NumberOfShares INT,
    @PositionSize INT,
    @CloseDate DATETIME = NULL,
    @AvgSellPrice DECIMAL(6, 2) = NULL,
    @IsClosed BIT,
    @Duration INT,
    @InitialDescription NVARCHAR(250) = NULL,
    @ClosingDescription NVARCHAR(250)
AS
BEGIN
    UPDATE [dbo].[Transactions]
    SET 
        CompanyID = @CompanyID, 
        CompanyName = @CompanyName, 
        EntryDate = @EntryDate, 
        EntryPrice = @EntryPrice, 
        EntryMedianVolume = @EntryMedianVolume, 
        NumberOfShares = @NumberOfShares, 
        PositionSize = @PositionSize, 
        CloseDate = @CloseDate, 
        AvgSellPrice = @AvgSellPrice, 
        IsClosed = @IsClosed, 
        Duration = @Duration, 
        InitialDescription = @InitialDescription, 
        ClosingDescription = @ClosingDescription
    WHERE ID = @ID;
END;
