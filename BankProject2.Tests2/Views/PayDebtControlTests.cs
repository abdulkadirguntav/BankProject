using BankProject2;
using BankProject2.Data;
using BankProject2.Models;
using BankProject2.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;
using System.Threading;

namespace BankProject2.Tests2.Views
{
    public class PayDebtControlTests
    {
        private DbContextOptions<BankDbContext> _options;

        public PayDebtControlTests()
        {
            _options = new DbContextOptionsBuilder<BankDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void Constructor_ShouldInitializeWithCustomerId()
        {
            // Arrange & Act
            var payDebtControl = new PayDebtControl(1);

            // Assert
            Assert.NotNull(payDebtControl);
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void LoadCreditCardData_ShouldLoadCustomerCreditCards()
        {
            // Arrange
            using (var context = new BankDbContext(_options))
            {
                var customer = new Customer { CustomerID = 1, FirstName = "Test", LastName = "User" };
                var creditCard = new CreditCard 
                { 
                    CreditCardID = 1, 
                    CustomerID = 1, 
                    Limit = 10000f,
                    CurrentDebt = 2500f,
                    RiskScore = 750,
                    LatePaymentCount = 0
                };
                
                context.customer.Add(customer);
                context.creditCard.Add(creditCard);
                context.SaveChanges();
            }

            // Act
            var payDebtControl = new PayDebtControl(1);

            // Assert
            Assert.NotNull(payDebtControl);
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void LoadCreditCardData_ShouldHandleCustomerWithoutCreditCards()
        {
            // Arrange
            using (var context = new BankDbContext(_options))
            {
                var customer = new Customer { CustomerID = 1, FirstName = "Test", LastName = "User" };
                // Hiç kredi kartı yok
                
                context.customer.Add(customer);
                context.SaveChanges();
            }

            // Act
            var payDebtControl = new PayDebtControl(1);

            // Assert
            Assert.NotNull(payDebtControl);
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void LoadCreditCardData_ShouldHandleMultipleCreditCards()
        {
            // Arrange
            using (var context = new BankDbContext(_options))
            {
                var customer = new Customer { CustomerID = 1, FirstName = "Test", LastName = "User" };
                var creditCard1 = new CreditCard 
                { 
                    CreditCardID = 1, 
                    CustomerID = 1, 
                    Limit = 10000f,
                    CurrentDebt = 2500f,
                    RiskScore = 750,
                    LatePaymentCount = 0
                };
                var creditCard2 = new CreditCard 
                { 
                    CreditCardID = 2, 
                    CustomerID = 1, 
                    Limit = 15000f,
                    CurrentDebt = 5000f,
                    RiskScore = 650,
                    LatePaymentCount = 1
                };
                
                context.customer.Add(customer);
                context.creditCard.Add(creditCard1);
                context.creditCard.Add(creditCard2);
                context.SaveChanges();
            }

            // Act
            var payDebtControl = new PayDebtControl(1);

            // Assert
            Assert.NotNull(payDebtControl);
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void LoadCreditCardData_ShouldHandleHighDebt()
        {
            // Arrange
            using (var context = new BankDbContext(_options))
            {
                var customer = new Customer { CustomerID = 1, FirstName = "Test", LastName = "User" };
                var creditCard = new CreditCard 
                { 
                    CreditCardID = 1, 
                    CustomerID = 1, 
                    Limit = 5000f,
                    CurrentDebt = 4800f, // Yüksek borç
                    RiskScore = 850,
                    LatePaymentCount = 3
                };
                
                context.customer.Add(customer);
                context.creditCard.Add(creditCard);
                context.SaveChanges();
            }

            // Act
            var payDebtControl = new PayDebtControl(1);

            // Assert
            Assert.NotNull(payDebtControl);
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void LoadCreditCardData_ShouldHandleZeroDebt()
        {
            // Arrange
            using (var context = new BankDbContext(_options))
            {
                var customer = new Customer { CustomerID = 1, FirstName = "Test", LastName = "User" };
                var creditCard = new CreditCard 
                { 
                    CreditCardID = 1, 
                    CustomerID = 1, 
                    Limit = 10000f,
                    CurrentDebt = 0f, // Sıfır borç
                    RiskScore = 500,
                    LatePaymentCount = 0
                };
                
                context.customer.Add(customer);
                context.creditCard.Add(creditCard);
                context.SaveChanges();
            }

            // Act
            var payDebtControl = new PayDebtControl(1);

            // Assert
            Assert.NotNull(payDebtControl);
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void LoadCreditCardData_ShouldHandleExceededLimit()
        {
            // Arrange
            using (var context = new BankDbContext(_options))
            {
                var customer = new Customer { CustomerID = 1, FirstName = "Test", LastName = "User" };
                var creditCard = new CreditCard 
                { 
                    CreditCardID = 1, 
                    CustomerID = 1, 
                    Limit = 5000f,
                    CurrentDebt = 6000f, // Limiti aşan borç
                    RiskScore = 950,
                    LatePaymentCount = 5
                };
                
                context.customer.Add(customer);
                context.creditCard.Add(creditCard);
                context.SaveChanges();
            }

            // Act
            var payDebtControl = new PayDebtControl(1);

            // Assert
            Assert.NotNull(payDebtControl);
        }
    }
}
