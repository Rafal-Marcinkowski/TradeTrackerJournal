CREATE TABLE [dbo].[TransactionEndOfDayPrices]
(
    [ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionID] INT FOREIGN KEY REFERENCES Transactions(ID) NOT NULL, 
    [DayIndex] INT NOT NULL,  
    [EndOfDayPrice] DECIMAL(8, 2) NOT NULL
);
