CREATE TABLE DailyData (
    ID INT PRIMARY KEY IDENTITY,      
    TransactionID INT FOREIGN KEY REFERENCES Transactions(ID) NOT NULL,            
    Date DATETIME NOT NULL,                
    OpenPrice DECIMAL(12, 2) NOT NULL,      
    ClosePrice DECIMAL(12, 2) NOT NULL,     
    Volume DECIMAL(12, 2) NOT NULL,        
    Turnover DECIMAL(12, 2) NOT NULL,        
    MinPrice DECIMAL(12, 2) NOT NULL,       
    MaxPrice DECIMAL(12, 2) NOT NULL,       
    PriceChange DECIMAL(12, 2)  NULL,   
    TurnoverChange DECIMAL(12, 2)  NULL, 
    [TransactionCount] INT NULL   
);