using Assert = Xunit.Assert;
using Helpers;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Domain.Tests.Common.ValueObjects;

[TestClass]
public class FullNameTests
{
    [Fact]
    public void Create_WithValidNames_ReturnsSuccessResult()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var result = FullName.Create(firstName, lastName);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(firstName, result.Value.FirstName);
        Assert.Equal(lastName, result.Value.LastName);
    }

    [Theory]
    [InlineData("", "Doe")]
    [InlineData(" ", "Doe")]
    [InlineData(null, "Doe")]
    public void Create_WithInvalidFirstName_ThrowsArgumentException(string firstName, string lastName)
    {
        // Act
        var result = FullName.Create(firstName, lastName);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'firstName' is required.", result.Error.Message);
    }

    [Theory]
    [InlineData("John", "")]
    [InlineData("John", " ")]
    [InlineData("John", null)]
    public void Create_WithInvalidLastName_ThrowsArgumentException(string firstName, string lastName)
    {
        // Act
        var result = FullName.Create(firstName, lastName);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'lastName' is required.", result.Error.Message);
    }

    [Theory]
    [InlineData(null, null)]
    public void Create_WithNullNames_ThrowsArgumentNullException(string firstName, string lastName)
    {
        // Act
        var result = FullName.Create(firstName, lastName);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'firstName' is required.", result.Error.Message);
        Assert.Contains("Value 'lastName' is required.", result.Error.Message);
    }

    [Fact]
    public void GetEqualityComponents_WithSameNames_ReturnsEqual()
    {
        // Arrange
        var fullName1 = FullName.Create("John", "Doe").Value;
        var fullName2 = FullName.Create("John", "Doe").Value;

        // Act & Assert
        Assert.Equal(fullName1, fullName2);
    }

    [Fact]
    public void GetEqualityComponents_WithDifferentFirstNames_ReturnsNotEqual()
    {
        // Arrange
        var fullName1 = FullName.Create("John", "Doe").Value;
        var fullName2 = FullName.Create("Jane", "Doe").Value;

        // Act & Assert
        Assert.NotEqual(fullName1, fullName2);
    }

    [Fact]
    public void GetEqualityComponents_WithDifferentLastNames_ReturnsNotEqual()
    {
        // Arrange
        var fullName1 = FullName.Create("John", "Doe").Value;
        var fullName2 = FullName.Create("John", "Smith").Value;

        // Act & Assert
        Assert.NotEqual(fullName1, fullName2);
    }
}
