CREATE PROCEDURE GetTransactionComment
    @ID INT
AS
BEGIN
    SELECT * FROM [dbo].[TransactionComments]
    WHERE ID = @ID;
END;
