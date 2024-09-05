CREATE PROCEDURE GetAllEventsForCompany
    @CompanyID INT
AS
BEGIN
    SELECT *
    FROM [dbo].[Events]
    WHERE CompanyID = @CompanyID;
END;
