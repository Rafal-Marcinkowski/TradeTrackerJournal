CREATE PROCEDURE InsertEvent
    @CompanyID INT,
    @CompanyName NVARCHAR(35),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(12, 2) = NULL,
    @InitialDescription NVARCHAR(250) = NULL,
    @InformationLink NVARCHAR(250) = NULL,
    @IsTracking BIT,
    @EntryMedianTurnover INT
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
        EntryMedianTurnover)
    VALUES (
        @CompanyID, 
        @CompanyName, 
        @EntryDate, 
        @EntryPrice,
        @InitialDescription, 
        @InformationLink,
        @IsTracking,
        @EntryMedianTurnover);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
