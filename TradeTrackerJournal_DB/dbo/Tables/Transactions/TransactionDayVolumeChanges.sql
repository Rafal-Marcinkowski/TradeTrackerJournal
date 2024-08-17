CREATE TABLE [dbo].[TransactionDayVolumeChanges]
(
	 [ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionID] INT FOREIGN KEY REFERENCES Transactions(ID) NOT NULL, 
    [DayIndex] INT NOT NULL,  
    [DayVolumeChange] DECIMAL(6, 2) NOT NULL
);
