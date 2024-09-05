CREATE PROCEDURE GetDailyDataForEvent
    @EventID int
AS
BEGIN
    SELECT * FROM DailyData WHERE EventID = @EventID;
END
