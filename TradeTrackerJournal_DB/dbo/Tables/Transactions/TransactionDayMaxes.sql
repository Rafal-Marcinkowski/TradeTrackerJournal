CREATE TABLE [dbo].[TransactionDayMaxes]
(
	[ [ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionID] INT FOREIGN KEY REFERENCES Transactions(ID) NOT NULL, 
    [DayIndex] INT NOT NULL,  
    [DayMaxes] DECIMAL(6, 2) NOT NULL
);
