CREATE TABLE DailyData (
    ID INT PRIMARY KEY IDENTITY,      
    TransactionID INT NOT NULL,            
    Date DATETIME NOT NULL,                
    OpenPrice DECIMAL(8, 2) NOT NULL,      
    ClosePrice DECIMAL(8, 2) NOT NULL,     
    Volume DECIMAL(8, 2) NOT NULL,        
    Turnover DECIMAL(8, 2) NOT NULL,        
    MinPrice DECIMAL(8, 2) NOT NULL,       
    MaxPrice DECIMAL(8, 2) NOT NULL,       
    PriceChange DECIMAL(8, 2)  NULL,   
    TurnoverChange DECIMAL(8, 2)  NULL, 
    [TransactionCount] INT NULL   
);