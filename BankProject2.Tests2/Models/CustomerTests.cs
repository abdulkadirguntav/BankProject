using BankProject2.Models;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Xunit;
using Moq;

public class CustomerTests
{
    [Fact]
    public void Customer_PropertyAssignment_WorksCorrectly()
    {
        // Arrange & Act
        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = "Ali",
            LastName = "Veli",
            PhoneNumber = "5551234567",
            CustomerPassword = "Sifre123!",
            MonthlyIncome = 15000.5f
        };

        // Assert
        Assert.Equal(1, customer.CustomerID);
        Assert.Equal("Ali", customer.FirstName);
        Assert.Equal("Veli", customer.LastName);
        Assert.Equal("5551234567", customer.PhoneNumber);
        Assert.Equal("Sifre123!", customer.CustomerPassword);
        Assert.Equal(15000.5f, customer.MonthlyIncome);
    }

    [Fact]
    public void CustomerID_ShouldBeMarkedAsKey()
    {
        // Arrange
        var property = typeof(Customer).GetProperty(nameof(Customer.CustomerID));

        // Act & Assert
        Assert.True(Attribute.IsDefined(property, typeof(KeyAttribute)));
    }

    [Fact]
    public void Customer_CanHandleEmptyPassword()
    {
        // Arrange & Act
        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "5551234567",
            CustomerPassword = "", // Boş şifre
            MonthlyIncome = 10000f
        };

        // Assert - Model validation olmadığı için boş şifre kabul edilir
        Assert.Equal("", customer.CustomerPassword);
    }

    [Fact]
    public void Customer_CanHandleInvalidPhoneNumber()
    {
        // Arrange & Act
        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "123", // Geçersiz telefon numarası
            CustomerPassword = "password123",
            MonthlyIncome = 10000f
        };

        // Assert - Model validation olmadığı için geçersiz telefon kabul edilir
        Assert.Equal("123", customer.PhoneNumber);
    }

    [Fact]
    public void Customer_CanHandleNullValues()
    {
        // Arrange & Act
        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = null,
            LastName = null,
            PhoneNumber = null,
            CustomerPassword = null,
            MonthlyIncome = 0f
        };

        // Assert
        Assert.Null(customer.FirstName);
        Assert.Null(customer.LastName);
        Assert.Null(customer.PhoneNumber);
        Assert.Null(customer.CustomerPassword);
    }

    [Fact]
    public void Customer_CanHandleVeryLongNames()
    {
        // Arrange & Act
        var longName = new string('A', 1000);
        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = longName,
            LastName = longName,
            PhoneNumber = "5551234567",
            CustomerPassword = "password123",
            MonthlyIncome = 10000f
        };

        // Assert - Model validation olmadığı için uzun isim kabul edilir
        Assert.Equal(longName, customer.FirstName);
        Assert.Equal(longName, customer.LastName);
    }

    [Fact]
    public void Customer_CanHandleNegativeIncome()
    {
        // Arrange & Act
        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "5551234567",
            CustomerPassword = "password123",
            MonthlyIncome = -5000f // Negatif gelir
        };

        // Assert - Model validation olmadığı için negatif gelir kabul edilir
        Assert.Equal(-5000f, customer.MonthlyIncome);
    }

    [Fact]
    public void Customer_CanHandleZeroIncome()
    {
        // Arrange & Act
        var customer = new Customer
        {
            CustomerID = 1,
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "5551234567",
            CustomerPassword = "password123",
            MonthlyIncome = 0f
        };

        // Assert
        Assert.Equal(0f, customer.MonthlyIncome);
    }
}