CREATE PROCEDURE InsertEvent
    @CompanyID INT,
    @CompanyName NVARCHAR(35),
    @EntryDate DATETIME,
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
        InitialDescription, 
        InformationLink,
        IsTracking,
        EntryMedianTurnover)
    VALUES (
        @CompanyID, 
        @CompanyName, 
        @EntryDate, 
        @InitialDescription, 
        @InformationLink,
        @IsTracking,
        @EntryMedianTurnover);
    
    SELECT SCOPE_IDENTITY() AS NewID;
END;
