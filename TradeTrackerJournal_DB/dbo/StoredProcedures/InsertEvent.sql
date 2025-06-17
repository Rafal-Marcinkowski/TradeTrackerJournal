CREATE PROCEDURE InsertEvent
    @CompanyID INT,
    @CompanyName NVARCHAR(35),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(12, 2) = NULL,
    @InitialDescription NVARCHAR(250) = NULL,
    @InformationLink NVARCHAR(250) = NULL,
    @IsTracking BIT,
    @EntryMedianTurnover INT,
    @Description NVARCHAR(4000) = NULL
AS
BEGIN
    INSERT INTO [dbo].[Events] (
        CompanyID, 
        CompanyName, 
        EntryDate, 
        EntryPrice,
        InitialDescription, 
        InformationLink,
        IsTracking,
        EntryMedianTurnover,
        Description)
    VALUES (
        @CompanyID, 
        @CompanyName, 
        @EntryDate, 
        @EntryPrice,
        @InitialDescription, 
        @InformationLink,
        @IsTracking,
        @EntryMedianTurnover,
        @Description);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
