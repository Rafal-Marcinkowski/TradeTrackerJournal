﻿CREATE PROCEDURE UpdateTransaction
    @ID INT,
    @CompanyID INT,
    @CompanyName NVARCHAR(35),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(12, 3) = NULL,
    @EntryMedianTurnover INT,
    @NumberOfShares INT,
    @PositionSize INT,
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
    UPDATE [dbo].[Transactions]
    SET 
        CompanyID = @CompanyID, 
        CompanyName = @CompanyName, 
        EntryDate = @EntryDate, 
        EntryPrice = @EntryPrice, 
        EntryMedianTurnover = @EntryMedianTurnover, 
        NumberOfShares = @NumberOfShares, 
        PositionSize = @PositionSize, 
        CloseDate = @CloseDate, 
        AvgSellPrice = @AvgSellPrice, 
        IsClosed = @IsClosed, 
        InitialDescription = @InitialDescription, 
        ClosingDescription = @ClosingDescription,
        InformationLink = @InformationLink,
        IsTracking = @IsTracking,
        Description=@Description
    WHERE ID = @ID;
END;
