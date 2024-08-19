CREATE PROCEDURE InsertTransaction
    @CompanyID INT,
    @CompanyName NVARCHAR(30),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(8, 2) = NULL,
    @EntryMedianVolume INT,
    @NumberOfShares INT,
    @PositionSize DECIMAL(8,2),
    @CloseDate DATETIME = NULL,
    @AvgSellPrice DECIMAL(8, 2) = NULL,
    @IsClosed BIT,
    @InitialDescription NVARCHAR(250) = NULL,
    @ClosingDescription NVARCHAR(250),
    @InformationLink NVARCHAR(150) = NULL
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
        InitialDescription, 
        ClosingDescription,
        InformationLink)
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
        @InitialDescription, 
        @ClosingDescription,
        @InformationLink);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
