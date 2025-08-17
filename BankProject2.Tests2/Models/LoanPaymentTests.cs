using BankProject2.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Moq;

public class LoanPaymentTests
{
    [Fact]
    public void LoanPayment_PropertyAssignment_WorksCorrectly()
    {
        // Arrange & Act
        var loanPayment = new LoanPayment
        {
            LoanPaymentID = 1,
            Amount = 1500f,
            PaidDate = new DateTime(2025, 8, 14)
        };

        // Assert
        Assert.Equal(1, loanPayment.LoanPaymentID);
        Assert.Equal(1500f, loanPayment.Amount);
        Assert.Equal(new DateTime(2025, 8, 14), loanPayment.PaidDate);
    }

    [Fact]
    public void LoanPaymentID_ShouldBeMarkedAsKey()
    {
        // Arrange
        var property = typeof(LoanPayment).GetProperty(nameof(LoanPayment.LoanPaymentID));

        // Act & Assert
        Assert.True(Attribute.IsDefined(property, typeof(KeyAttribute)));
    }

    [Fact]
    public void LoanPayment_CanHandleNegativeAmount()
    {
        // Arrange & Act
        var loanPayment = new LoanPayment
        {
            LoanPaymentID = 1,
            Amount = -500f, // Negatif ödeme tutarı
            PaidDate = DateTime.Now
        };

        // Assert - Model validation olmadığı için negatif tutar kabul edilir
        Assert.Equal(-500f, loanPayment.Amount);
    }

    [Fact]
    public void LoanPayment_CanHandleFutureDate()
    {
        // Arrange & Act
        var futureDate = DateTime.Now.AddDays(30);
        var loanPayment = new LoanPayment
        {
            LoanPaymentID = 1,
            Amount = 1000f,
            PaidDate = futureDate // Gelecek tarih
        };

        // Assert - Model validation olmadığı için gelecek tarih kabul edilir
        Assert.Equal(futureDate.Date, loanPayment.PaidDate.Date);
    }

    [Fact]
    public void LoanPayment_CanHandleZeroAmount()
    {
        // Arrange & Act
        var loanPayment = new LoanPayment
        {
            LoanPaymentID = 1,
            Amount = 0f,
            PaidDate = DateTime.Now
        };

        // Assert
        Assert.Equal(0f, loanPayment.Amount);
    }

    [Fact]
    public void LoanPayment_CanHandleVeryHighAmount()
    {
        // Arrange & Act
        var loanPayment = new LoanPayment
        {
            LoanPaymentID = 1,
            Amount = 999999999.99f,
            PaidDate = DateTime.Now
        };

        // Assert
        Assert.Equal(999999999.99f, loanPayment.Amount);
    }
}