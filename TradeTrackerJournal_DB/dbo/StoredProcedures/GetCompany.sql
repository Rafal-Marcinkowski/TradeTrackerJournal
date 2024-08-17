CREATE PROCEDURE GetCompany
    @ID INT
AS
BEGIN
    SELECT * FROM [dbo].[Companies]
    WHERE ID = @ID;
END;
