using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankProject2.Models
{
    internal class DataBase
    {
        /*
-- 2️⃣ OLUŞTUR
CREATE DATABASE Bank;
USE Bank;

-- 3️⃣ MÜŞTERİ
CREATE TABLE Customer (
    CustomerID INT NOT NULL AUTO_INCREMENT,
    FirstName VARCHAR(255) NOT NULL,
    LastName VARCHAR(255) NOT NULL,
    PhoneNumber VARCHAR(20) UNIQUE,
    CustomerPassword VARCHAR(255) NOT NULL,
    MonthlyIncome DOUBLE DEFAULT 0, -- opsiyonel: limit kontrolünde kullanılabilir
    PRIMARY KEY (CustomerID)
);

-- 4️⃣ DÖVİZ
CREATE TABLE Currency (
    CurrencyID INT NOT NULL AUTO_INCREMENT,
    CurrencyCode VARCHAR(10) NOT NULL,
    CurrencyDate DATE,
    RateToTRY DOUBLE,
    PRIMARY KEY (CurrencyID)
);

-- 5️⃣ HESAPLAR
CREATE TABLE Accounts (
    AccountID INT NOT NULL AUTO_INCREMENT,
    IBAN VARCHAR(34) UNIQUE,
    AccountType VARCHAR(50) NOT NULL,  -- Vadesiz, Vadeli, Döviz
    Balance DOUBLE DEFAULT 0,
    StartDate DATE DEFAULT NULL,       -- Vadeli
    MaturityDate DATE DEFAULT NULL,    -- Vadeli
    InterestRate DOUBLE DEFAULT NULL,  -- Vadeli
    IsBroken BOOLEAN DEFAULT FALSE,    -- Vadeli
    PrincipalAmount DOUBLE DEFAULT NULL, -- Vadeli anapara
    AccruedInterest DOUBLE DEFAULT NULL, -- Vadeli işlenen faiz
    CurrencyID INT,
    CustomerID INT,
    PRIMARY KEY (AccountID),
    FOREIGN KEY (CurrencyID) REFERENCES Currency(CurrencyID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- 6️⃣ KREDİ KARTI
CREATE TABLE CreditCard (
    CreditCardID INT NOT NULL AUTO_INCREMENT,
    CustomerID INT,
    CardNumber VARCHAR(20) UNIQUE,
    CardExpiry DATE,
    CardCVV VARCHAR(4),
    `Limit` DOUBLE DEFAULT 0,     -- Kart limiti
    CurrentDebt DOUBLE DEFAULT 0, -- Borç
    RiskScore INT DEFAULT 0,      -- Kart risk skoru
    LatePaymentCount INT DEFAULT 0,
    PRIMARY KEY (CreditCardID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- 7️⃣ İŞLEMLER
CREATE TABLE Transactions (
    TransactionID INT NOT NULL AUTO_INCREMENT,
    FromAccountID INT,
    ToAccountID INT,
    CreditCardID INT, -- Kart işlemi varsa
    Amount DOUBLE,
    TransactionType VARCHAR(50), -- Deposit, Withdraw, Transfer, FX-Buy, FX-Sell, CardPayment, CardRepayment
    TransactionDate DATE,
    Fee DOUBLE,
    Description VARCHAR(255),
    PRIMARY KEY (TransactionID),
    FOREIGN KEY (FromAccountID) REFERENCES Accounts(AccountID),
    FOREIGN KEY (ToAccountID) REFERENCES Accounts(AccountID),
    FOREIGN KEY (CreditCardID) REFERENCES CreditCard(CreditCardID)
);

-- 8️⃣ KREDİ (İSTERSEN KALSIN)
CREATE TABLE Loan (
    LoanID INT NOT NULL AUTO_INCREMENT,
    LoanStatus VARCHAR(50) NOT NULL,
    InterestRate DOUBLE,
    Principal DOUBLE,
    TermMonths INT,
    CustomerID INT,
    PRIMARY KEY (LoanID),
    FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);

-- 9️⃣ KREDİ ÖDEMELERİ (İSTERSEN KALSIN)
CREATE TABLE LoanPayment (
    LoanPaymentID INT NOT NULL AUTO_INCREMENT,
    Amount DOUBLE,
    PaidDate DATE,
    LoanID INT,
    PRIMARY KEY (LoanPaymentID),
    FOREIGN KEY (LoanID) REFERENCES Loan(LoanID)
);

        */
    }
}
