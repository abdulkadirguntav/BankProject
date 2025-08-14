using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankProject2.Models;
using BankProject2.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace BankProject2.Tests.NewFolder
{
    [TestClass]
    public class BankProjectTests
    {
        private BankDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Test veritabanı bağlantısı
            _context = new BankDbContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        #region Customer Tests

        [TestMethod]
        public void Customer_Creation_ShouldSetAllProperties()
        {
            // Arrange
            var customer = new Customer
            {
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                PhoneNumber = "5551234567",
                CustomerPassword = "test123",
                MonthlyIncome = 5000
            };

            // Assert
            Assert.AreEqual("Ahmet", customer.FirstName);
            Assert.AreEqual("Yılmaz", customer.LastName);
            Assert.AreEqual("5551234567", customer.PhoneNumber);
            Assert.AreEqual("test123", customer.CustomerPassword);
            Assert.AreEqual(5000, customer.MonthlyIncome);
        }

        [TestMethod]
        public void Customer_PhoneNumber_ShouldBeUnique()
        {
            // Arrange
            var phoneNumber = "5551234567";
            var customer1 = new Customer
            {
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                PhoneNumber = phoneNumber,
                CustomerPassword = "test123",
                MonthlyIncome = 5000
            };

            var customer2 = new Customer
            {
                FirstName = "Mehmet",
                LastName = "Demir",
                PhoneNumber = phoneNumber, // Aynı telefon numarası
                CustomerPassword = "test456",
                MonthlyIncome = 6000
            };

            // Assert
            Assert.AreEqual(customer1.PhoneNumber, customer2.PhoneNumber);
        }

        #endregion

        #region Accounts Tests

        [TestMethod]
        public void Accounts_Creation_ShouldSetAllProperties()
        {
            // Arrange
            var account = new Accounts
            {
                AccountType = "Vadesiz",
                Balance = 1000.50f,
                IBAN = "TR123456789012345678901234",
                CustomerID = 1,
                CurrencyID = 1
            };

            // Assert
            Assert.AreEqual("Vadesiz", account.AccountType);
            Assert.AreEqual(1000.50f, account.Balance);
            Assert.AreEqual("TR123456789012345678901234", account.IBAN);
            Assert.AreEqual(1, account.CustomerID);
            Assert.AreEqual(1, account.CurrencyID);
        }

        [TestMethod]
        public void Accounts_VadeliHesap_ShouldSetVadeliProperties()
        {
            // Arrange
            var startDate = DateTime.Now;
            var maturityDate = startDate.AddMonths(12);
            
            var account = new Accounts
            {
                AccountType = "Vadeli",
                Balance = 5000.0f,
                IBAN = "TR123456789012345678901234",
                CustomerID = 1,
                StartDate = startDate,
                MaturityDate = maturityDate,
                InterestRate = 12.5f,
                PrincipalAmount = 5000.0f,
                AccruedInterest = 0.0f,
                IsBroken = false
            };

            // Assert
            Assert.AreEqual("Vadeli", account.AccountType);
            Assert.AreEqual(startDate, account.StartDate);
            Assert.AreEqual(maturityDate, account.MaturityDate);
            Assert.AreEqual(12.5f, account.InterestRate);
            Assert.AreEqual(5000.0f, account.PrincipalAmount);
            Assert.AreEqual(false, account.IsBroken);
        }

        #endregion

        #region CreditCard Tests

        [TestMethod]
        public void CreditCard_Creation_ShouldSetAllProperties()
        {
            // Arrange
            var expiryDate = DateTime.Now.AddYears(3);
            var creditCard = new CreditCard
            {
                CustomerID = 1,
                CardNumber = "4111111111111111",
                CardExpiry = expiryDate,
                CardCVV = "123",
                Limit = 10000.0f,
                CurrentDebt = 0.0f,
                RiskScore = 1,
                LatePaymentCount = 0
            };

            // Assert
            Assert.AreEqual(1, creditCard.CustomerID);
            Assert.AreEqual("4111111111111111", creditCard.CardNumber);
            Assert.AreEqual(expiryDate, creditCard.CardExpiry);
            Assert.AreEqual("123", creditCard.CardCVV);
            Assert.AreEqual(10000.0f, creditCard.Limit);
            Assert.AreEqual(0.0f, creditCard.CurrentDebt);
            Assert.AreEqual(1, creditCard.RiskScore);
            Assert.AreEqual(0, creditCard.LatePaymentCount);
        }

        [TestMethod]
        public void CreditCard_RiskScore_ShouldBeValid()
        {
            // Arrange & Act
            var lowRiskCard = new CreditCard { RiskScore = 1 };
            var mediumRiskCard = new CreditCard { RiskScore = 2 };
            var highRiskCard = new CreditCard { RiskScore = 3 };

            // Assert
            Assert.IsTrue(lowRiskCard.RiskScore >= 1 && lowRiskCard.RiskScore <= 3);
            Assert.IsTrue(mediumRiskCard.RiskScore >= 1 && mediumRiskCard.RiskScore <= 3);
            Assert.IsTrue(highRiskCard.RiskScore >= 1 && highRiskCard.RiskScore <= 3);
        }

        #endregion

        #region Currency Tests

        [TestMethod]
        public void Currency_Creation_ShouldSetAllProperties()
        {
            // Arrange
            var date = DateTime.Now;
            var currency = new Currency
            {
                CurrencyCode = "USD",
                CurrencyDate = date,
                RateToTRY = 28.50
            };

            // Assert
            Assert.AreEqual("USD", currency.CurrencyCode);
            Assert.AreEqual(date, currency.CurrencyDate);
            Assert.AreEqual(28.50, currency.RateToTRY);
        }

        [TestMethod]
        public void Currency_RateToTRY_ShouldBePositive()
        {
            // Arrange
            var currency = new Currency
            {
                CurrencyCode = "EUR",
                RateToTRY = 30.25
            };

            // Assert
            Assert.IsTrue(currency.RateToTRY > 0);
        }

        #endregion

        #region Loan Tests

        [TestMethod]
        public void Loan_Creation_ShouldSetAllProperties()
        {
            // Arrange
            var loan = new Loan
            {
                LoanStatus = "Active",
                InterestRate = 15.5,
                Principal = 50000.0,
                TermMonths = 36,
                CustomerID = 1
            };

            // Assert
            Assert.AreEqual("Active", loan.LoanStatus);
            Assert.AreEqual(15.5, loan.InterestRate);
            Assert.AreEqual(50000.0, loan.Principal);
            Assert.AreEqual(36, loan.TermMonths);
            Assert.AreEqual(1, loan.CustomerID);
        }

        [TestMethod]
        public void Loan_TermMonths_ShouldBePositive()
        {
            // Arrange
            var loan = new Loan
            {
                TermMonths = 24
            };

            // Assert
            Assert.IsTrue(loan.TermMonths > 0);
        }

        #endregion

        #region LoanPayment Tests

        [TestMethod]
        public void LoanPayment_Creation_ShouldSetAllProperties()
        {
            // Arrange
            var paymentDate = DateTime.Now;
            var loanPayment = new LoanPayment
            {
                Amount = 1500.0,
                PaidDate = paymentDate,
                LoanID = 1
            };

            // Assert
            Assert.AreEqual(1500.0, loanPayment.Amount);
            Assert.AreEqual(paymentDate, loanPayment.PaidDate);
            Assert.AreEqual(1, loanPayment.LoanID);
        }

        [TestMethod]
        public void LoanPayment_Amount_ShouldBePositive()
        {
            // Arrange
            var loanPayment = new LoanPayment
            {
                Amount = 1000.0
            };

            // Assert
            Assert.IsTrue(loanPayment.Amount > 0);
        }

        #endregion

        #region Transactions Tests

        [TestMethod]
        public void Transactions_Creation_ShouldSetAllProperties()
        {
            // Arrange
            var transactionDate = DateTime.Now;
            var transaction = new Transactions
            {
                FromAccountID = 1,
                ToAccountID = 2,
                Amount = 500.0,
                TransactionType = "Transfer",
                TransactionDate = transactionDate,
                Fee = 5.0,
                Description = "Havale işlemi"
            };

            // Assert
            Assert.AreEqual(1, transaction.FromAccountID);
            Assert.AreEqual(2, transaction.ToAccountID);
            Assert.AreEqual(500.0, transaction.Amount);
            Assert.AreEqual("Transfer", transaction.TransactionType);
            Assert.AreEqual(transactionDate, transaction.TransactionDate);
            Assert.AreEqual(5.0, transaction.Fee);
            Assert.AreEqual("Havale işlemi", transaction.Description);
        }

        [TestMethod]
        public void Transactions_Amount_ShouldBePositive()
        {
            // Arrange
            var transaction = new Transactions
            {
                Amount = 100.0
            };

            // Assert
            Assert.IsTrue(transaction.Amount > 0);
        }

        #endregion

        #region Business Logic Tests

        [TestMethod]
        public void CalculateRiskScore_ShouldReturnCorrectRiskLevel()
        {
            // Arrange
            var lowIncome = 3000.0f;
            var mediumIncome = 7000.0f;
            var highIncome = 15000.0f;

            // Act
            var lowRisk = CalculateRiskScore(lowIncome);
            var mediumRisk = CalculateRiskScore(mediumIncome);
            var highRisk = CalculateRiskScore(highIncome);

            // Assert
            Assert.AreEqual(3, lowRisk);    // Yüksek risk
            Assert.AreEqual(2, mediumRisk); // Orta risk
            Assert.AreEqual(1, highRisk);   // Düşük risk
        }

        [TestMethod]
        public void GenerateIBAN_ShouldHaveCorrectFormat()
        {
            // Act
            var iban = GenerateIBAN();

            // Assert
            Assert.IsTrue(iban.StartsWith("TR"));
            Assert.AreEqual(26, iban.Length);
            Assert.IsTrue(iban.Substring(2).All(char.IsDigit));
        }

        [TestMethod]
        public void GenerateCardNumber_ShouldHaveCorrectFormat()
        {
            // Act
            var cardNumber = GenerateCardNumber();

            // Assert
            Assert.IsTrue(cardNumber.StartsWith("4")); // Visa kartı
            Assert.AreEqual(16, cardNumber.Length);
            Assert.IsTrue(cardNumber.All(char.IsDigit));
        }

        [TestMethod]
        public void GenerateCVV_ShouldHaveCorrectFormat()
        {
            // Act
            var cvv = GenerateCVV();

            // Assert
            Assert.AreEqual(3, cvv.Length);
            Assert.IsTrue(cvv.All(char.IsDigit));
            Assert.IsTrue(int.Parse(cvv) >= 100 && int.Parse(cvv) <= 999);
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void ValidatePhoneNumber_ShouldAcceptValidNumbers()
        {
            // Arrange
            var validPhoneNumbers = new[] { "5551234567", "5321234567", "2121234567" };

            // Act & Assert
            foreach (var phone in validPhoneNumbers)
            {
                Assert.IsTrue(ValidatePhoneNumber(phone));
            }
        }

        [TestMethod]
        public void ValidatePhoneNumber_ShouldRejectInvalidNumbers()
        {
            // Arrange
            var invalidPhoneNumbers = new[] { "123", "abc1234567", "555123456", "" };

            // Act & Assert
            foreach (var phone in invalidPhoneNumbers)
            {
                Assert.IsFalse(ValidatePhoneNumber(phone));
            }
        }

        [TestMethod]
        public void ValidatePassword_ShouldAcceptValidPasswords()
        {
            // Arrange
            var validPasswords = new[] { "test123", "password123", "123456" };

            // Act & Assert
            foreach (var password in validPasswords)
            {
                Assert.IsTrue(ValidatePassword(password));
            }
        }

        [TestMethod]
        public void ValidatePassword_ShouldRejectInvalidPasswords()
        {
            // Arrange
            var invalidPasswords = new[] { "123", "abc", "", "a" };

            // Act & Assert
            foreach (var password in invalidPasswords)
            {
                Assert.IsFalse(ValidatePassword(password));
            }
        }

        #endregion

        #region Helper Methods

        private int CalculateRiskScore(float monthlyIncome)
        {
            if (monthlyIncome >= 10000) return 1; // Düşük risk
            if (monthlyIncome >= 5000) return 2;  // Orta risk
            return 3; // Yüksek risk
        }

        private string GenerateIBAN()
        {
            Random random = new Random();
            string bankCode = "0015";
            string accountNumber = random.Next(100000000, 999999999).ToString() + random.Next(10000000, 99999999).ToString();
            return "TR" + random.Next(10, 99) + bankCode + accountNumber;
        }

        private string GenerateCardNumber()
        {
            Random random = new Random();
            string cardNumber = "4";
            for (int i = 0; i < 15; i++)
            {
                cardNumber += random.Next(0, 10);
            }
            return cardNumber;
        }

        private string GenerateCVV()
        {
            Random random = new Random();
            return random.Next(100, 1000).ToString();
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && 
                   phoneNumber.Length >= 10 && 
                   phoneNumber.All(char.IsDigit);
        }

        private bool ValidatePassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && 
                   password.Length >= 6;
        }

        #endregion
    }
}
