CREATE PROCEDURE DeleteTransactionComment
    @ID INT
AS
BEGIN
    DELETE FROM [dbo].[TransactionComments]
    WHERE ID = @ID;
END;
