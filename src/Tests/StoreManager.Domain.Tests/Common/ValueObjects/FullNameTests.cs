using Assert = Xunit.Assert;
using Helpers;
using StoreManager.Domain.Common.ValueObjects;

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
    public void Create_WithInvalidFirstName_ThrowsArgumentException(string firstName, string lastName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => FullName.Create(firstName, lastName));
    }

    [Theory]
    [InlineData("John", "")]
    [InlineData("John", " ")]
    public void Create_WithInvalidLastName_ThrowsArgumentException(string firstName, string lastName)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => FullName.Create(firstName, lastName));
    }

    [Theory]
    [InlineData(null, "Doe")]
    [InlineData("John", null)]
    [InlineData(null, null)]
    public void Create_WithNullNames_ThrowsArgumentNullException(string firstName, string lastName)
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FullName.Create(firstName, lastName));
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
