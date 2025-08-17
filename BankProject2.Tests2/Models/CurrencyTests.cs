using BankProject2.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Moq;

public class CurrencyTests
{
    [Fact]
    public void Currency_PropertyAssignment_WorksCorrectly()
    {
        // Arrange & Act
        var currency = new Currency
        {
            CurrencyID = 1,
            CurrencyCode = "USD",
            RateToTRY = 32.15
        };

        // Assert
        Assert.Equal(1, currency.CurrencyID);
        Assert.Equal("USD", currency.CurrencyCode);
        Assert.Equal(32.15, currency.RateToTRY);
    }

    [Fact]
    public void CurrencyID_ShouldBeMarkedAsKey()
    {
        // Arrange
        var property = typeof(Currency).GetProperty(nameof(Currency.CurrencyID));

        // Act & Assert
        Assert.True(Attribute.IsDefined(property, typeof(KeyAttribute)));
    }

    [Fact]
    public void Currency_CanHandleNegativeRate()
    {
        // Arrange & Act
        var currency = new Currency
        {
            CurrencyID = 1,
            CurrencyCode = "TEST",
            RateToTRY = -1.5 // Negatif kur
        };

        // Assert - Model validation olmadığı için negatif kur kabul edilir
        Assert.Equal(-1.5, currency.RateToTRY);
    }

    [Fact]
    public void Currency_CanHandleZeroRate()
    {
        // Arrange & Act
        var currency = new Currency
        {
            CurrencyID = 1,
            CurrencyCode = "ZERO",
            RateToTRY = 0.0
        };

        // Assert
        Assert.Equal(0.0, currency.RateToTRY);
    }

    [Fact]
    public void Currency_CanHandleVeryHighRate()
    {
        // Arrange & Act
        var currency = new Currency
        {
            CurrencyID = 1,
            CurrencyCode = "HIGH",
            RateToTRY = 999999.99
        };

        // Assert
        Assert.Equal(999999.99, currency.RateToTRY);
    }

    [Fact]
    public void Currency_CanHandleNullValues()
    {
        // Arrange & Act
        var currency = new Currency
        {
            CurrencyID = 1,
            CurrencyCode = null,
            RateToTRY = 0.0
        };

        // Assert
        Assert.Null(currency.CurrencyCode);
    }

    [Fact]
    public void Currency_CanHandleLongCurrencyCode()
    {
        // Arrange & Act
        var longCode = new string('A', 100);
        var currency = new Currency
        {
            CurrencyID = 1,
            CurrencyCode = longCode,
            RateToTRY = 1.0
        };

        // Assert - Model validation olmadığı için uzun kod kabul edilir
        Assert.Equal(longCode, currency.CurrencyCode);
    }
}