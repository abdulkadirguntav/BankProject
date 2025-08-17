using BankProject2.Models;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Moq;

public class LoanTests
{
    [Fact]
    public void Loan_PropertyAssignment_WorksCorrectly()
    {
        // Arrange & Act
        var loan = new Loan
        {
            LoanID = 1,
            InterestRate = 0.12f,
            Principal = 50000f,
            TermMonths = 36,
            LoanStatus = "Active"
        };

        // Assert
        Assert.Equal(1, loan.LoanID);
        Assert.Equal(0.12f, loan.InterestRate);
        Assert.Equal(50000f, loan.Principal);
        Assert.Equal(36, loan.TermMonths);
        Assert.Equal("Active", loan.LoanStatus);
    }

    [Fact]
    public void LoanID_ShouldBeMarkedAsKey()
    {
        // Arrange
        var property = typeof(Loan).GetProperty(nameof(Loan.LoanID));

        // Act & Assert
        Assert.True(Attribute.IsDefined(property, typeof(KeyAttribute)));
    }

    [Fact]
    public void Loan_CanHandleNegativeInterestRate()
    {
        // Arrange & Act
        var loan = new Loan
        {
            LoanID = 1,
            InterestRate = -0.05f, // Negatif faiz oranı
            Principal = 10000f,
            TermMonths = 12,
            LoanStatus = "Special"
        };

        // Assert - Model validation olmadığı için negatif faiz kabul edilir
        Assert.Equal(-0.05f, loan.InterestRate);
    }

    [Fact]
    public void Loan_CanHandleShortTermMonths()
    {
        // Arrange & Act
        var loan = new Loan
        {
            LoanID = 1,
            InterestRate = 0.08f,
            Principal = 5000f,
            TermMonths = 6, // Kısa vade (12 aydan az)
            LoanStatus = "ShortTerm"
        };

        // Assert - Model validation olmadığı için kısa vade kabul edilir
        Assert.Equal(6, loan.TermMonths);
        Assert.True(loan.TermMonths < 12);
    }

    [Fact]
    public void Loan_CanHandleVeryHighInterestRate()
    {
        // Arrange & Act
        var loan = new Loan
        {
            LoanID = 1,
            InterestRate = 2.5f, // Çok yüksek faiz oranı (%250)
            Principal = 1000f,
            TermMonths = 60,
            LoanStatus = "HighRisk"
        };

        // Assert
        Assert.Equal(2.5f, loan.InterestRate);
        Assert.True(loan.InterestRate > 1.0f);
    }

    [Fact]
    public void Loan_CanHandleZeroValues()
    {
        // Arrange & Act
        var loan = new Loan
        {
            LoanID = 1,
            InterestRate = 0f,
            Principal = 0f,
            TermMonths = 0,
            LoanStatus = "Pending"
        };

        // Assert
        Assert.Equal(0f, loan.InterestRate);
        Assert.Equal(0f, loan.Principal);
        Assert.Equal(0, loan.TermMonths);
    }

    [Fact]
    public void Loan_CanHandleNullLoanStatus()
    {
        // Arrange & Act
        var loan = new Loan
        {
            LoanID = 1,
            InterestRate = 0.10f,
            Principal = 20000f,
            TermMonths = 24,
            LoanStatus = null
        };

        // Assert
        Assert.Null(loan.LoanStatus);
    }

    [Fact]
    public void Loan_CanHandleVeryLongTerm()
    {
        // Arrange & Act
        var loan = new Loan
        {
            LoanID = 1,
            InterestRate = 0.06f,
            Principal = 100000f,
            TermMonths = 360, // 30 yıl
            LoanStatus = "LongTerm"
        };

        // Assert
        Assert.Equal(360, loan.TermMonths);
        Assert.True(loan.TermMonths > 300);
    }
}