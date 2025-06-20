CREATE TABLE DailyData (
    ID INT PRIMARY KEY IDENTITY,      
    TransactionID INT FOREIGN KEY REFERENCES Transactions(ID) NULL,            
    EventID INT FOREIGN KEY REFERENCES Events(ID) NULL,            
    Date DATETIME NOT NULL,                
    OpenPrice DECIMAL(12, 3) NOT NULL,      
    ClosePrice DECIMAL(12, 3) NOT NULL,     
    Volume DECIMAL(12, 2) NOT NULL,        
    Turnover DECIMAL(12, 2) NOT NULL,        
    MinPrice DECIMAL(12, 3) NOT NULL,       
    MaxPrice DECIMAL(12, 3) NOT NULL,       
    PriceChange DECIMAL(12, 2)  NULL,   
    TurnoverChange DECIMAL(12, 2)  NULL, 
    [TransactionCount] INT NULL   
);