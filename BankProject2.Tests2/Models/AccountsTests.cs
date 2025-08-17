using BankProject2.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Moq;
public class AccountsTests
{
    [Fact]
    public void Account_PropertyAssignment_WorksCorrectly()
    {
        // Arrange & Act
        var account = new Accounts
        {
            AccountID = 1,
            AccountType = "Vadesiz",
            Balance = 1000.50f,
            IBAN = "TR123456789",
            CurrencyID = 1,
            StartDate = new DateTime(2025, 1, 1),
            MaturityDate = new DateTime(2025, 12, 31),
            InterestRate = 5.5f,
            IsBroken = false,
            PrincipalAmount = 1000f,
            AccruedInterest = 55.5f,
            CustomerID = 10
        };

        // Assert
        Assert.Equal(1, account.AccountID);
        Assert.Equal("Vadesiz", account.AccountType);
        Assert.Equal(1000.50f, account.Balance);
        Assert.Equal("TR123456789", account.IBAN);
        Assert.Equal(new DateTime(2025, 1, 1), account.StartDate);
        Assert.Equal(new DateTime(2025, 12, 31), account.MaturityDate);
        Assert.Equal(5.5f, account.InterestRate);
        Assert.False(account.IsBroken ?? true);
        Assert.Equal(1000f, account.PrincipalAmount);
        Assert.Equal(55.5f, account.AccruedInterest);
        Assert.Equal(10, account.CustomerID);
    }

    [Fact]
    public void AccountID_ShouldBeMarkedAsKey()
    {
        var property = typeof(Accounts).GetProperty("AccountID");
        var attribute = property.GetCustomAttributes(typeof(KeyAttribute), inherit: false);

        Assert.NotEmpty(attribute);
    }
}