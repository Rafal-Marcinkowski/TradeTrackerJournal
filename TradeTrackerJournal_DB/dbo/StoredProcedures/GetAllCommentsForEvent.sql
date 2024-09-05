CREATE PROCEDURE GetAllCommentsForEvent
    @EventID INT
AS
BEGIN
    SELECT *
    FROM [dbo].[Comments]
    WHERE EventID = @EventID;
END;
