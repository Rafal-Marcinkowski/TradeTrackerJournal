CREATE PROCEDURE DeleteCompany
    @ID INT
AS
BEGIN
    DELETE FROM [dbo].[Companies]
    WHERE ID = @ID;
END;
