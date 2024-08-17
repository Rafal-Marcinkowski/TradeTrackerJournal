CREATE PROCEDURE InsertTransaction
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
    INSERT INTO [dbo].[Transactions] (
        CompanyID, 
        CompanyName, 
        EntryDate, 
        EntryPrice, 
        EntryMedianVolume, 
        NumberOfShares, 
        PositionSize, 
        CloseDate, 
        AvgSellPrice, 
        IsClosed, 
        Duration, 
        InitialDescription, 
        ClosingDescription)
    VALUES (
        @CompanyID, 
        @CompanyName, 
        @EntryDate, 
        @EntryPrice, 
        @EntryMedianVolume, 
        @NumberOfShares, 
        @PositionSize, 
        @CloseDate, 
        @AvgSellPrice, 
        @IsClosed, 
        @Duration, 
        @InitialDescription, 
        @ClosingDescription);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
