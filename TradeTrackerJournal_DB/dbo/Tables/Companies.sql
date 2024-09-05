CREATE TABLE [dbo].[Companies]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CompanyName] NVARCHAR(35) NOT NULL, 
    [TransactionCount] INT NOT NULL, 
    [EventCount] INT NULL
)
