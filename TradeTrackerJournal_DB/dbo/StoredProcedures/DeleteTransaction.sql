CREATE PROCEDURE DeleteTransaction
    @ID INT
AS
BEGIN
    DELETE FROM [dbo].[Transactions]
    WHERE ID = @ID;
END;
