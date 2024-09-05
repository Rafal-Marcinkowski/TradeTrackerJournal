CREATE PROCEDURE GetComment
    @ID INT
AS
BEGIN
    SELECT * FROM [dbo].[Comments]
    WHERE ID = @ID;
END;
