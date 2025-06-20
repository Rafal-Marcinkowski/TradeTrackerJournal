CREATE PROCEDURE InsertTransaction
    @CompanyID INT,
    @CompanyName NVARCHAR(35),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(12, 3) = NULL,
    @EntryMedianTurnover INT,
    @NumberOfShares INT,
    @PositionSize DECIMAL(12,2),
    @CloseDate DATETIME = NULL,
    @AvgSellPrice DECIMAL(12, 3) = NULL,
    @IsClosed BIT,
    @InitialDescription NVARCHAR(250) = NULL,
    @ClosingDescription NVARCHAR(250),
    @InformationLink NVARCHAR(250) = NULL,
    @IsTracking BIT,
    @Description NVARCHAR(4000) = NULL
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
        IsTracking,
        Description)
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
        @IsTracking,
        @Description);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
