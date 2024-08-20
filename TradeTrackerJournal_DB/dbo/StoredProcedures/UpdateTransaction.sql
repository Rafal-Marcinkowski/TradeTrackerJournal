CREATE PROCEDURE UpdateTransaction
    @ID INT,
    @CompanyID INT,
    @CompanyName NVARCHAR(30),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(8, 2) = NULL,
    @EntryMedianVolume INT,
    @NumberOfShares INT,
    @PositionSize INT,
    @CloseDate DATETIME = NULL,
    @AvgSellPrice DECIMAL(8, 2) = NULL,
    @IsClosed BIT,
    @InitialDescription NVARCHAR(250) = NULL,
    @ClosingDescription NVARCHAR(250),
    @InformationLink NVARCHAR(150) = NULL,
    @IsTracking BIT
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
        InitialDescription = @InitialDescription, 
        ClosingDescription = @ClosingDescription,
        InformationLink = @InformationLink,
        IsTracking = @IsTracking
    WHERE ID = @ID;
END;
