CREATE PROCEDURE InsertTransaction
    @CompanyID INT,
    @CompanyName NVARCHAR(30),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(8, 2) = NULL,
    @EntryMedianTurnover INT,
    @NumberOfShares INT,
    @PositionSize DECIMAL(8,2),
    @CloseDate DATETIME = NULL,
    @AvgSellPrice DECIMAL(8, 2) = NULL,
    @IsClosed BIT,
    @InitialDescription NVARCHAR(250) = NULL,
    @ClosingDescription NVARCHAR(250),
    @InformationLink NVARCHAR(150) = NULL,
    @IsTracking BIT
AS
BEGIN
    INSERT INTO [dbo].[Transactions] (
        CompanyID, 
        CompanyName, 
        EntryDate, 
        EntryPrice, 
        EntryMedianTurnover, 
        NumberOfShares, 
        PositionSize, 
        CloseDate, 
        AvgSellPrice, 
        IsClosed, 
        InitialDescription, 
        ClosingDescription,
        InformationLink,
        IsTracking)
    VALUES (
        @CompanyID, 
        @CompanyName, 
        @EntryDate, 
        @EntryPrice, 
        @EntryMedianTurnover, 
        @NumberOfShares, 
        @PositionSize, 
        @CloseDate, 
        @AvgSellPrice, 
        @IsClosed, 
        @InitialDescription, 
        @ClosingDescription,
        @InformationLink,
        @IsTracking);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
