using Assert = Xunit.Assert;
using Helpers;
using StoreManager.Domain.Common.ValueObjects;

namespace StoreManager.Domain.Tests.Common.ValueObjects;

[TestClass]
public class PhoneNumberTests
{
    [Fact]
    public void Create_WithValidCountryCodeAndNumber_ReturnsSuccessResult()
    {
        // Arrange
        var countryCode = "45";
        var number = "12345678";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("+45", result.Value.CountryCode);
        Assert.Equal("12345678", result.Value.Number);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhiteSpaceNumber_ReturnsFailureResult(string number)
    {
        // Arrange
        var countryCode = "45";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
    }

    [Theory]
    [InlineData("abc123")]
    [InlineData("12-34-56")]
    [InlineData("123abc")]
    [InlineData("phone")]
    public void Create_WithNonNumericNumber_ReturnsFailureResult(string number)
    {
        // Arrange
        var countryCode = "45";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
    }

    [Fact]
    public void Create_WithNumberContainingWhitespace_TrimsAndValidates()
    {
        // Arrange
        var countryCode = "1";
        var number = "  5551234567  ";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("  5551234567  ", result.Value.Number);
    }

    [Fact]
    public void GetEqualityComponents_TwoPhoneNumbersWithSameValues_AreEqual()
    {
        // Arrange
        var phoneNumber1 = PhoneNumber.Create("45", "12345678").Value;
        var phoneNumber2 = PhoneNumber.Create("45", "12345678").Value;

        // Act & Assert
        Assert.Equal(phoneNumber1, phoneNumber2);
    }

    [Fact]
    public void GetEqualityComponents_TwoPhoneNumbersWithDifferentNumbers_AreNotEqual()
    {
        // Arrange
        var phoneNumber1 = PhoneNumber.Create("45", "12345678").Value;
        var phoneNumber2 = PhoneNumber.Create("45", "87654321").Value;

        // Act & Assert
        Assert.NotEqual(phoneNumber1, phoneNumber2);
    }

    [Fact]
    public void GetEqualityComponents_TwoPhoneNumbersWithDifferentCountryCodes_AreNotEqual()
    {
        // Arrange
        var phoneNumber1 = PhoneNumber.Create("45", "12345678").Value;
        var phoneNumber2 = PhoneNumber.Create("46", "12345678").Value;

        // Act & Assert
        Assert.NotEqual(phoneNumber1, phoneNumber2);
    }
}
