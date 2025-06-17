CREATE TABLE [dbo].[Events]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CompanyID] INT FOREIGN KEY REFERENCES Companies(ID) NOT NULL, 
    [CompanyName] NVARCHAR(35) NOT NULL, 
    [EntryDate] DATETIME NOT NULL, 
    [EntryPrice] DECIMAL(12, 2) NOT NULL, 
    [InitialDescription] NVARCHAR(250) NULL, 
    [InformationLink] NVARCHAR(250) NULL, 
    [IsTracking] BIT NOT NULL, 
    [EntryMedianTurnover] INT NOT NULL,
    [Description] NVARCHAR(4000) NULL
)
