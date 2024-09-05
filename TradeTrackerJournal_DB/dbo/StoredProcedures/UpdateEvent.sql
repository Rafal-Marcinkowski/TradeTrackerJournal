CREATE PROCEDURE UpdateEvent
    @ID INT,
    @CompanyID INT,
    @CompanyName NVARCHAR(35),
    @InitialDescription NVARCHAR(250) = NULL,
    @InformationLink NVARCHAR(250) = NULL,
    @IsTracking BIT,
    @EntryMedianTurnover INT
AS
BEGIN
    UPDATE [dbo].[Events]
    SET 
        CompanyID = @CompanyID, 
        CompanyName = @CompanyName, 
        InitialDescription = @InitialDescription, 
        InformationLink = @InformationLink,
        IsTracking = @IsTracking,
        EntryMedianTurnover = @EntryMedianTurnover
    WHERE ID = @ID;
END;
