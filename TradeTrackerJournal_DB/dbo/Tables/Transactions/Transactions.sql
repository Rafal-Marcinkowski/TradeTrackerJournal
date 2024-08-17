CREATE TABLE [dbo].[Transactions]
(
	[ID] INT NOT NULL PRIMARY KEY, 
    [CompanyID] INT FOREIGN KEY REFERENCES Companies(ID) NOT NULL, 
    [CompanyName ] NVARCHAR(30) NOT NULL, 
    [EntryDate] DATETIME NOT NULL, 
    [EntryPrice] DECIMAL(6, 2) NULL, 
    [EntryMedianVolume] INT NOT NULL, 
    [NumberOfShares] INT NOT NULL, 
    [PositionSize] INT NOT NULL, 
    [CloseDate] DATETIME NULL, 
    [AvgSellPrice] DECIMAL(6, 2) NULL, 
    [IsClosed] BIT NOT NULL, 
    [Duration] INT NOT NULL, 
    [InitialDescription] NVARCHAR(250) NULL, 
    [ClosingDescription] NVARCHAR(250) NOT NULL
);
