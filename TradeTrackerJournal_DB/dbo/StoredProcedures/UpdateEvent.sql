CREATE PROCEDURE UpdateEvent
    @ID INT,
    @CompanyID INT,
    @CompanyName NVARCHAR(35),
    @EntryDate DATETIME,
    @EntryPrice DECIMAL(12, 2) = NULL,
    @InitialDescription NVARCHAR(250) = NULL,
    @InformationLink NVARCHAR(250) = NULL,
    @IsTracking BIT,
    @EntryMedianTurnover INT,
    @Description NVARCHAR(2000) = NULL
AS
BEGIN
    UPDATE [dbo].[Events]
    SET 
        CompanyID = @CompanyID, 
        CompanyName = @CompanyName, 
        EntryDate = @EntryDate, 
        EntryPrice = @EntryPrice, 
        InitialDescription = @InitialDescription, 
        InformationLink = @InformationLink,
        IsTracking = @IsTracking,
        EntryMedianTurnover = @EntryMedianTurnover,
        Description = @Description
    WHERE ID = @ID;
END;
