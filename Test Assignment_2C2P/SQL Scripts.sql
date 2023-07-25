CREATE DATABASE [UPD];

CREATE TABLE TransactionUPD (
    TransactionId VARCHAR(50) NOT NULL,
    Amount decimal (18, 2) NOT NULL,
	CurrencyCode VARCHAR(3) NOT NULL,
	TransactionDate datetime NOT NULL,
	Status VARCHAR(20) NOT NULL
);
