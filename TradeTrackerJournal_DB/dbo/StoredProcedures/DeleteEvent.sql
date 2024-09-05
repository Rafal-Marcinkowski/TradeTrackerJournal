CREATE PROCEDURE DeleteEvent
    @ID INT
AS
BEGIN
    DELETE FROM [dbo].[Events]
    WHERE ID = @ID;
END;
