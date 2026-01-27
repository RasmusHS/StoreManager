using Assert = Xunit.Assert;
using Helpers;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Domain.Tests.Common.ValueObjects;

[TestClass]
public class AddressTests
{
    [Fact]
    public void Create_WithValidParameters_ReturnsSuccessResult()
    {
        // Arrange
        var street = "123 Main St";
        var postalCode = "12345";
        var city = "Springfield";

        // Act
        var result = Address.Create(street, postalCode, city);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(street, result.Value.Street);
        Assert.Equal(postalCode, result.Value.PostalCode);
        Assert.Equal(city, result.Value.City);
    }

    [Theory]
    [InlineData("", "12345", "Springfield")]
    [InlineData("   ", "12345", "Springfield")]
    [InlineData(null, "12345", "Springfield")]
    public void Create_WithInvalidStreet_ThrowsArgumentException(string street, string postalCode, string city)
    {
        // Act
        var result = Address.Create(street, postalCode, city);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'street' is required.", result.Error.Message);
    }

    [Theory]
    [InlineData("123 Main St", "", "Springfield")]
    [InlineData("123 Main St", "   ", "Springfield")]
    [InlineData("123 Main St", null, "Springfield")]
    public void Create_WithInvalidPostalCode_ThrowsArgumentException(string street, string postalCode, string city)
    {
        // Act
        var result = Address.Create(street, postalCode, city);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'postalCode' is required.", result.Error.Message);
    }

    [Theory]
    [InlineData("123 Main St", "12345", "")]
    [InlineData("123 Main St", "12345", "   ")]
    [InlineData("123 Main St", "12345", null)]
    public void Create_WithInvalidCity_ThrowsArgumentException(string street, string postalCode, string city)
    {
        // Act
        var result = Address.Create(street, postalCode, city);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'city' is required.", result.Error.Message);
    }

    [Theory]
    [InlineData(null, "12345", null)]
    [InlineData("123 Main St", null, null)]
    [InlineData(null, null, "Springfield")]
    [InlineData(null, null, null)]
    public void Create_WithNullValues_ThrowsArgumentNullException(string street, string postalCode, string city)
    {
        // Act
        var result = Address.Create(street, postalCode, city);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        if (street == null)
            Assert.Contains("Value 'street' is required.", result.Error.Message);
        if (postalCode == null)
            Assert.Contains("Value 'postalCode' is required.", result.Error.Message);
        if (city == null)
            Assert.Contains("Value 'city' is required.", result.Error.Message);
    }

    [Fact]
    public void GetEqualityComponents_WithSameValues_AreEqual()
    {
        // Arrange
        var address1 = Address.Create("123 Main St", "12345", "Springfield").Value;
        var address2 = Address.Create("123 Main St", "12345", "Springfield").Value;

        // Act & Assert
        Assert.Equal(address1, address2);
    }

    [Fact]
    public void GetEqualityComponents_WithDifferentStreets_AreNotEqual()
    {
        // Arrange
        var address1 = Address.Create("123 Main St", "12345", "Springfield").Value;
        var address2 = Address.Create("456 Oak Ave", "12345", "Springfield").Value;

        // Act & Assert
        Assert.NotEqual(address1, address2);
    }

    [Fact]
    public void GetEqualityComponents_WithDifferentPostalCodes_AreNotEqual()
    {
        // Arrange
        var address1 = Address.Create("123 Main St", "12345", "Springfield").Value;
        var address2 = Address.Create("123 Main St", "54321", "Springfield").Value;

        // Act & Assert
        Assert.NotEqual(address1, address2);
    }

    [Fact]
    public void GetEqualityComponents_WithDifferentCities_AreNotEqual()
    {
        // Arrange
        var address1 = Address.Create("123 Main St", "12345", "Springfield").Value;
        var address2 = Address.Create("123 Main St", "12345", "Shelbyville").Value;

        // Act & Assert
        Assert.NotEqual(address1, address2);
    }
}
