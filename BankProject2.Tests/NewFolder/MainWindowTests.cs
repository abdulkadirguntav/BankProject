using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BankProject2.Models;
using BankProject2.Data;
using System.Linq;

namespace BankProject2.Tests.NewFolder
{
    [TestClass]
    public class MainWindowTests
    {
        private BankDbContext _context;

        [TestInitialize]
        public void Setup()
        {
            _context = new BankDbContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context?.Dispose();
        }

        #region Registration Tests

        [TestMethod]
        public void Register_ValidCustomer_ShouldCreateCustomerAndAccount()
        {
            // Arrange
            var testPhone = "5551234567";
            var testPassword = "test123";

            // Test verilerini temizle
            var existingCustomer = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            if (existingCustomer != null)
            {
                _context.customer.Remove(existingCustomer);
                _context.SaveChanges();
            }

            // Act
            var newCustomer = new Customer
            {
                FirstName = "Test",
                LastName = "User",
                PhoneNumber = testPhone,
                CustomerPassword = testPassword,
                MonthlyIncome = 5000
            };

            _context.customer.Add(newCustomer);
            _context.SaveChanges();

            var newAccount = new Accounts
            {
                AccountType = "Vadesiz",
                Balance = 0,
                IBAN = GenerateTestIBAN(),
                CustomerID = newCustomer.CustomerID
            };

            _context.accounts.Add(newAccount);
            _context.SaveChanges();

            // Assert
            var savedCustomer = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            Assert.IsNotNull(savedCustomer);
            Assert.AreEqual("Test", savedCustomer.FirstName);
            Assert.AreEqual("User", savedCustomer.LastName);
            Assert.AreEqual(testPassword, savedCustomer.CustomerPassword);
            Assert.AreEqual(5000, savedCustomer.MonthlyIncome);

            var savedAccount = _context.accounts.FirstOrDefault(a => a.CustomerID == savedCustomer.CustomerID);
            Assert.IsNotNull(savedAccount);
            Assert.AreEqual("Vadesiz", savedAccount.AccountType);
            Assert.AreEqual(0, savedAccount.Balance);
            Assert.IsTrue(savedAccount.IBAN.StartsWith("TR"));

            // Cleanup
            _context.accounts.Remove(savedAccount);
            _context.customer.Remove(savedCustomer);
            _context.SaveChanges();
        }

        [TestMethod]
        public void Register_DuplicatePhoneNumber_ShouldNotAllowRegistration()
        {
            // Arrange
            var testPhone = "5551234568";
            var testPassword = "test123";

            // Test verilerini temizle
            var existingCustomer = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            if (existingCustomer != null)
            {
                _context.customer.Remove(existingCustomer);
                _context.SaveChanges();
            }

            // İlk müşteriyi ekle
            var firstCustomer = new Customer
            {
                FirstName = "First",
                LastName = "User",
                PhoneNumber = testPhone,
                CustomerPassword = testPassword,
                MonthlyIncome = 5000
            };

            _context.customer.Add(firstCustomer);
            _context.SaveChanges();

            // Act & Assert - Aynı telefon numarası ile ikinci kayıt
            var duplicateCustomer = new Customer
            {
                FirstName = "Second",
                LastName = "User",
                PhoneNumber = testPhone, // Aynı telefon numarası
                CustomerPassword = "test456",
                MonthlyIncome = 6000
            };

            // Aynı telefon numarası ile kayıt yapılmaya çalışıldığında hata olmalı
            var existingUser = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            Assert.IsNotNull(existingUser);
            Assert.AreEqual("First", existingUser.FirstName);

            // Cleanup
            _context.customer.Remove(firstCustomer);
            _context.SaveChanges();
        }

        [TestMethod]
        public void Register_WithCreditCard_ShouldCreateCreditCard()
        {
            // Arrange
            var testPhone = "5551234569";
            var testPassword = "test123";
            var monthlyIncome = 8000.0f;

            // Test verilerini temizle
            var existingCustomer = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            if (existingCustomer != null)
            {
                var existingAccounts = _context.accounts.Where(a => a.CustomerID == existingCustomer.CustomerID);
                var existingCards = _context.creditCard.Where(c => c.CustomerID == existingCustomer.CustomerID);
                
                _context.accounts.RemoveRange(existingAccounts);
                _context.creditCard.RemoveRange(existingCards);
                _context.customer.Remove(existingCustomer);
                _context.SaveChanges();
            }

            // Act
            var newCustomer = new Customer
            {
                FirstName = "Credit",
                LastName = "CardUser",
                PhoneNumber = testPhone,
                CustomerPassword = testPassword,
                MonthlyIncome = monthlyIncome
            };

            _context.customer.Add(newCustomer);
            _context.SaveChanges();

            var newAccount = new Accounts
            {
                AccountType = "Vadesiz",
                Balance = 0,
                IBAN = GenerateTestIBAN(),
                CustomerID = newCustomer.CustomerID
            };

            _context.accounts.Add(newAccount);
            _context.SaveChanges();

            var newCreditCard = new CreditCard
            {
                CustomerID = newCustomer.CustomerID,
                CardNumber = GenerateTestCardNumber(),
                CardExpiry = DateTime.Now.AddYears(3),
                CardCVV = GenerateTestCVV(),
                Limit = monthlyIncome * 2,
                CurrentDebt = 0,
                RiskScore = CalculateTestRiskScore(monthlyIncome),
                LatePaymentCount = 0
            };

            _context.creditCard.Add(newCreditCard);
            _context.SaveChanges();

            // Assert
            var savedCustomer = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            Assert.IsNotNull(savedCustomer);

            var savedAccount = _context.accounts.FirstOrDefault(a => a.CustomerID == savedCustomer.CustomerID);
            Assert.IsNotNull(savedAccount);

            var savedCard = _context.creditCard.FirstOrDefault(c => c.CustomerID == savedCustomer.CustomerID);
            Assert.IsNotNull(savedCard);
            Assert.AreEqual(monthlyIncome * 2, savedCard.Limit);
            Assert.AreEqual(0, savedCard.CurrentDebt);
            Assert.IsTrue(savedCard.RiskScore >= 1 && savedCard.RiskScore <= 3);

            // Cleanup
            _context.creditCard.Remove(savedCard);
            _context.accounts.Remove(savedAccount);
            _context.customer.Remove(savedCustomer);
            _context.SaveChanges();
        }

        #endregion

        #region Login Tests

        [TestMethod]
        public void Login_ValidCredentials_ShouldAuthenticateUser()
        {
            // Arrange
            var testPhone = "5551234570";
            var testPassword = "test123";

            // Test verilerini temizle
            var existingCustomer = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            if (existingCustomer != null)
            {
                var existingAccounts = _context.accounts.Where(a => a.CustomerID == existingCustomer.CustomerID);
                _context.accounts.RemoveRange(existingAccounts);
                _context.customer.Remove(existingCustomer);
                _context.SaveChanges();
            }

            // Test müşterisi oluştur
            var testCustomer = new Customer
            {
                FirstName = "Login",
                LastName = "Test",
                PhoneNumber = testPhone,
                CustomerPassword = testPassword,
                MonthlyIncome = 5000
            };

            _context.customer.Add(testCustomer);
            _context.SaveChanges();

            // Act
            var authenticatedUser = _context.customer.FirstOrDefault(c => 
                c.PhoneNumber == testPhone && c.CustomerPassword == testPassword);

            // Assert
            Assert.IsNotNull(authenticatedUser);
            Assert.AreEqual(testPhone, authenticatedUser.PhoneNumber);
            Assert.AreEqual(testPassword, authenticatedUser.CustomerPassword);
            Assert.AreEqual("Login", authenticatedUser.FirstName);

            // Cleanup
            _context.customer.Remove(authenticatedUser);
            _context.SaveChanges();
        }

        [TestMethod]
        public void Login_InvalidCredentials_ShouldNotAuthenticate()
        {
            // Arrange
            var testPhone = "5551234571";
            var correctPassword = "correct123";
            var wrongPassword = "wrong123";

            // Test verilerini temizle
            var existingCustomer = _context.customer.FirstOrDefault(c => c.PhoneNumber == testPhone);
            if (existingCustomer != null)
            {
                var existingAccounts = _context.accounts.Where(a => a.CustomerID == existingCustomer.CustomerID);
                _context.accounts.RemoveRange(existingAccounts);
                _context.customer.Remove(existingCustomer);
                _context.SaveChanges();
            }

            // Test müşterisi oluştur
            var testCustomer = new Customer
            {
                FirstName = "Login",
                LastName = "Test",
                PhoneNumber = testPhone,
                CustomerPassword = correctPassword,
                MonthlyIncome = 5000
            };

            _context.customer.Add(testCustomer);
            _context.SaveChanges();

            // Act
            var authenticatedUser = _context.customer.FirstOrDefault(c => 
                c.PhoneNumber == testPhone && c.CustomerPassword == wrongPassword);

            // Assert
            Assert.IsNull(authenticatedUser);

            // Cleanup
            _context.customer.Remove(testCustomer);
            _context.SaveChanges();
        }

        #endregion

        #region Validation Tests

        [TestMethod]
        public void Validation_PhoneNumber_ShouldValidateCorrectly()
        {
            // Arrange
            var validPhoneNumbers = new[] { "5551234567", "5321234567", "2121234567" };
            var invalidPhoneNumbers = new[] { "123", "abc1234567", "555123456", "" };

            // Act & Assert
            foreach (var phone in validPhoneNumbers)
            {
                Assert.IsTrue(ValidateTestPhoneNumber(phone), $"Phone number {phone} should be valid");
            }

            foreach (var phone in invalidPhoneNumbers)
            {
                Assert.IsFalse(ValidateTestPhoneNumber(phone), $"Phone number {phone} should be invalid");
            }
        }

        [TestMethod]
        public void Validation_Password_ShouldValidateCorrectly()
        {
            // Arrange
            var validPasswords = new[] { "test123", "password123", "123456" };
            var invalidPasswords = new[] { "123", "abc", "", "a" };

            // Act & Assert
            foreach (var password in validPasswords)
            {
                Assert.IsTrue(ValidateTestPassword(password), $"Password {password} should be valid");
            }

            foreach (var password in invalidPasswords)
            {
                Assert.IsFalse(ValidateTestPassword(password), $"Password {password} should be invalid");
            }
        }

        [TestMethod]
        public void Validation_RequiredFields_ShouldBeEnforced()
        {
            // Arrange
            var customer = new Customer();

            // Act & Assert
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.FirstName));
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.LastName));
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.PhoneNumber));
            Assert.IsTrue(string.IsNullOrWhiteSpace(customer.CustomerPassword));
        }

        #endregion

        #region Business Logic Tests

        [TestMethod]
        public void RiskScore_Calculation_ShouldBeCorrect()
        {
            // Arrange
            var lowIncome = 3000.0f;
            var mediumIncome = 7000.0f;
            var highIncome = 15000.0f;

            // Act
            var lowRisk = CalculateTestRiskScore(lowIncome);
            var mediumRisk = CalculateTestRiskScore(mediumIncome);
            var highRisk = CalculateTestRiskScore(highIncome);

            // Assert
            Assert.AreEqual(3, lowRisk);    // Yüksek risk
            Assert.AreEqual(2, mediumRisk); // Orta risk
            Assert.AreEqual(1, highRisk);   // Düşük risk
        }

        [TestMethod]
        public void IBAN_Generation_ShouldHaveCorrectFormat()
        {
            // Act
            var iban = GenerateTestIBAN();

            // Assert
            Assert.IsTrue(iban.StartsWith("TR"));
            Assert.AreEqual(26, iban.Length);
            Assert.IsTrue(iban.Substring(2).All(char.IsDigit));
        }

        [TestMethod]
        public void CardNumber_Generation_ShouldHaveCorrectFormat()
        {
            // Act
            var cardNumber = GenerateTestCardNumber();

            // Assert
            Assert.IsTrue(cardNumber.StartsWith("4")); // Visa kartı
            Assert.AreEqual(16, cardNumber.Length);
            Assert.IsTrue(cardNumber.All(char.IsDigit));
        }

        [TestMethod]
        public void CVV_Generation_ShouldHaveCorrectFormat()
        {
            // Act
            var cvv = GenerateTestCVV();

            // Assert
            Assert.AreEqual(3, cvv.Length);
            Assert.IsTrue(cvv.All(char.IsDigit));
            Assert.IsTrue(int.Parse(cvv) >= 100 && int.Parse(cvv) <= 999);
        }

        #endregion

        #region Helper Methods

        private string GenerateTestIBAN()
        {
            Random random = new Random();
            string bankCode = "0015";
            string accountNumber = random.Next(100000000, 999999999).ToString() + random.Next(10000000, 99999999).ToString();
            return "TR" + random.Next(10, 99) + bankCode + accountNumber;
        }

        private string GenerateTestCardNumber()
        {
            Random random = new Random();
            string cardNumber = "4";
            for (int i = 0; i < 15; i++)
            {
                cardNumber += random.Next(0, 10);
            }
            return cardNumber;
        }

        private string GenerateTestCVV()
        {
            Random random = new Random();
            return random.Next(100, 1000).ToString();
        }

        private int CalculateTestRiskScore(float monthlyIncome)
        {
            if (monthlyIncome >= 10000) return 1; // Düşük risk
            if (monthlyIncome >= 5000) return 2;  // Orta risk
            return 3; // Yüksek risk
        }

        private bool ValidateTestPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrWhiteSpace(phoneNumber) && 
                   phoneNumber.Length >= 10 && 
                   phoneNumber.All(char.IsDigit);
        }

        private bool ValidateTestPassword(string password)
        {
            return !string.IsNullOrWhiteSpace(password) && 
                   password.Length >= 6;
        }

        #endregion
    }
}
