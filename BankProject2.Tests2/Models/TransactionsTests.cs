using BankProject2.Models;
using System;
using Xunit;
using Moq;

public class TransactionsTests
{
    [Fact]
    public void Transaction_PropertyAssignment_WorksCorrectly()
    {
        // Arrange & Act
        var transaction = new Transactions
        {
            TransactionID = 1,
            TransactionType = "Transfer",
            TransactionDate = new DateTime(2025, 8, 14, 14, 30, 0),
            FromAccountID = 100,
            ToAccountID = 200,
            Amount = 2500f,
            Fee = 15.5,
            Description = "Para transferi"
        };

        // Assert
        Assert.Equal(1, transaction.TransactionID);
        Assert.Equal("Transfer", transaction.TransactionType);
        Assert.Equal(new DateTime(2025, 8, 14), transaction.TransactionDate.Date);
        Assert.Equal(100, transaction.FromAccountID);
        Assert.Equal(200, transaction.ToAccountID);
        Assert.Equal(2500f, transaction.Amount);
        Assert.Equal(15.5, transaction.Fee);
        Assert.Equal("Para transferi", transaction.Description);
    }

    [Fact]
    public void Transaction_CanHandleNegativeAmount()
    {
        // Arrange & Act
        var transaction = new Transactions
        {
            TransactionID = 1,
            TransactionType = "Withdrawal",
            TransactionDate = DateTime.Now,
            Amount = -100f, // Negatif tutar (çekim)
            Fee = 5.0,
            Description = "Para çekme"
        };

        // Assert - Model validation olmadığı için negatif değer kabul edilir
        Assert.Equal(-100f, transaction.Amount);
    }

    [Fact]
    public void Transaction_CanHandleFutureDate()
    {
        // Arrange & Act
        var futureDate = DateTime.Now.AddDays(1);
        var transaction = new Transactions
        {
            TransactionID = 1,
            TransactionType = "Scheduled Transfer",
            TransactionDate = futureDate,
            Amount = 1000f,
            Fee = 10.0,
            Description = "Planlanmış transfer"
        };

        // Assert - Model validation olmadığı için gelecek tarih kabul edilir
        Assert.Equal(futureDate.Date, transaction.TransactionDate.Date);
    }

    [Fact]
    public void Transaction_CanHandleLongTransactionType()
    {
        // Arrange & Act
        var longType = new string('A', 100); // Uzun transaction type
        var transaction = new Transactions
        {
            TransactionID = 1,
            TransactionType = longType,
            TransactionDate = DateTime.Now,
            Amount = 100f,
            Fee = 5.0,
            Description = "Test transaction"
        };

        // Assert - Model validation olmadığı için uzun string kabul edilir
        Assert.Equal(longType, transaction.TransactionType);
    }

    [Fact]
    public void Transaction_CanHandleNullValues()
    {
        // Arrange & Act
        var transaction = new Transactions
        {
            TransactionID = 1,
            TransactionType = null,
            TransactionDate = DateTime.Now,
            FromAccountID = null,
            ToAccountID = null,
            CreditCardID = null,
            Description = null,
            Amount = 0f,
            Fee = 0.0
        };

        // Assert
        Assert.Null(transaction.TransactionType);
        Assert.Null(transaction.FromAccountID);
        Assert.Null(transaction.ToAccountID);
        Assert.Null(transaction.CreditCardID);
        Assert.Null(transaction.Description);
    }
}