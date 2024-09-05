CREATE PROCEDURE DeleteComment
    @ID INT
AS
BEGIN
    DELETE FROM [dbo].[Comments]
    WHERE ID = @ID;
END;
