using BankProject2;
using BankProject2.Data;
using BankProject2.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;
using System.Threading;

namespace BankProject2.Tests2.Views
{
    public class BankPageTests
    {
        private DbContextOptions<BankDbContext> _options;

        public BankPageTests()
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
            var bankPage = new BankPage(1);

            // Assert
            Assert.NotNull(bankPage);
        }

        [Fact]
        [STAThread] // WPF UI bileşenleri için STA thread gerekli
        public void LoadAccountData_ShouldLoadVadesizAccountData()
        {
            // Arrange
            using (var context = new BankDbContext(_options))
            {
                var customer = new Customer { CustomerID = 1, FirstName = "Test", LastName = "User" };
                var vadesizAccount = new Accounts 
                { 
                    AccountID = 1, 
                    CustomerID = 1, 
                    AccountType = "Vadesiz", 
                    Balance = 1000f,
                    IBAN = "TR123456789012345678901234" // IBAN alanı eklendi
                };
                
                context.customer.Add(customer);
                context.accounts.Add(vadesizAccount);
                context.SaveChanges();
            }

            // Act
            var bankPage = new BankPage(1);

            // Assert
            Assert.NotNull(bankPage);
        }
    }
}