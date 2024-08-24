CREATE TABLE [dbo].[Transactions]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CompanyID] INT FOREIGN KEY REFERENCES Companies(ID) NOT NULL, 
    [CompanyName] NVARCHAR(30) NOT NULL, 
    [EntryDate] DATETIME NOT NULL, 
    [EntryPrice] DECIMAL(12, 2) NOT NULL, 
    [EntryMedianTurnover] INT NOT NULL, 
    [NumberOfShares] INT NOT NULL, 
    [PositionSize] DECIMAL(12, 2) NOT NULL, 
    [CloseDate] DATETIME NULL, 
    [AvgSellPrice] DECIMAL(12, 2) NULL, 
    [IsClosed] BIT NOT NULL, 
    [InitialDescription] NVARCHAR(250) NULL, 
    [ClosingDescription] NVARCHAR(250) NULL, 
    [InformationLink] NVARCHAR(150) NULL, 
    [IsTracking] BIT NOT NULL
);
