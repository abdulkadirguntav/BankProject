using BankProject2.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Xunit;
using Moq;

public class CreditCardTests
{
    [Fact]
    public void CreditCard_PropertyAssignment_WorksCorrectly()
    {
        // Arrange & Act
        var creditCard = new CreditCard
        {
            CreditCardID = 1,
            CustomerID = 100,
            Limit = 10000f,
            CurrentDebt = 2500f,
            RiskScore = 750,
            LatePaymentCount = 0
        };

        // Assert
        Assert.Equal(1, creditCard.CreditCardID);
        Assert.Equal(100, creditCard.CustomerID);
        Assert.Equal(10000f, creditCard.Limit);
        Assert.Equal(2500f, creditCard.CurrentDebt);
        Assert.Equal(750, creditCard.RiskScore);
        Assert.Equal(0, creditCard.LatePaymentCount);
    }

    [Fact]
    public void CreditCardID_ShouldBeMarkedAsKey()
    {
        // Arrange
        var property = typeof(CreditCard).GetProperty(nameof(CreditCard.CreditCardID));

        // Act & Assert
        Assert.True(Attribute.IsDefined(property, typeof(KeyAttribute)));
    }

    [Fact]
    public void CreditCard_CanHandleCurrentDebtExceedingLimit()
    {
        // Arrange & Act
        var creditCard = new CreditCard
        {
            CreditCardID = 1,
            CustomerID = 100,
            Limit = 5000f,
            CurrentDebt = 6000f, // Limiti aşan borç
            RiskScore = 850,
            LatePaymentCount = 2
        };

        // Assert - Model validation olmadığı için limit aşımı kabul edilir
        Assert.Equal(5000f, creditCard.Limit);
        Assert.Equal(6000f, creditCard.CurrentDebt);
        Assert.True(creditCard.CurrentDebt > creditCard.Limit);
    }

    [Fact]
    public void CreditCard_CanHandleNullValues()
    {
        // Arrange & Act
        var creditCard = new CreditCard
        {
            CreditCardID = 1,
            CustomerID = 100,
            Limit = null,
            CurrentDebt = null,
            RiskScore = null,
            LatePaymentCount = null
        };

        // Assert
        Assert.Null(creditCard.Limit);
        Assert.Null(creditCard.CurrentDebt);
        Assert.Null(creditCard.RiskScore);
        Assert.Null(creditCard.LatePaymentCount);
    }

    [Fact]
    public void CreditCard_CanHandleHighRiskScore()
    {
        // Arrange & Act
        var creditCard = new CreditCard
        {
            CreditCardID = 1,
            CustomerID = 100,
            Limit = 2000f,
            CurrentDebt = 1800f,
            RiskScore = 950, // Yüksek risk skoru
            LatePaymentCount = 5
        };

        // Assert
        Assert.Equal(950, creditCard.RiskScore);
        Assert.Equal(5, creditCard.LatePaymentCount);
    }

    [Fact]
    public void CreditCard_CanHandleZeroValues()
    {
        // Arrange & Act
        var creditCard = new CreditCard
        {
            CreditCardID = 1,
            CustomerID = 100,
            Limit = 0f,
            CurrentDebt = 0f,
            RiskScore = 0,
            LatePaymentCount = 0
        };

        // Assert
        Assert.Equal(0f, creditCard.Limit);
        Assert.Equal(0f, creditCard.CurrentDebt);
        Assert.Equal(0, creditCard.RiskScore);
        Assert.Equal(0, creditCard.LatePaymentCount);
    }
}