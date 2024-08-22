CREATE TABLE [dbo].[TransactionComments]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionID] INT FOREIGN KEY REFERENCES Transactions(ID) NOT NULL, 
    [EntryDate] DATETIME NOT NULL,
    [CommentText] NVARCHAR(250) NOT NULL, 
)
