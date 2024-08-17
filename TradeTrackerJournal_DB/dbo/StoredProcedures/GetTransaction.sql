CREATE PROCEDURE GetTransaction
    @ID INT
AS
BEGIN
    SELECT * FROM [dbo].[Transactions]
    WHERE ID = @ID;
END;
