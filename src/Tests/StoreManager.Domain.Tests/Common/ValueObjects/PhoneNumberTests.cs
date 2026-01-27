using Helpers;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using Assert = Xunit.Assert;

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

    [Fact]
    public void Create_WithCountryCodeContainingPlus_RemovesPlusAndSucceeds()
    {
        // Arrange
        var countryCode = "+45";
        var number = "12345678";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("+45", result.Value.CountryCode);
        Assert.Equal("12345678", result.Value.Number);
    }

    [Fact]
    public void Create_WithCountryCodeContainingParentheses_RemovesParenthesesAndSucceeds()
    {
        // Arrange
        var countryCode = "(45)";
        var number = "12345678";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("+45", result.Value.CountryCode);
        Assert.Equal("12345678", result.Value.Number);
    }

    [Fact]
    public void Create_WithCountryCodeContainingWhitespace_TrimsAndSucceeds()
    {
        // Arrange
        var countryCode = "  45  ";
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
    public void Create_WithNullOrWhiteSpaceCountryCode_ReturnsFailureResult(string countryCode)
    {
        // Arrange
        var number = "12345678";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'countryCode' is required.", result.Error.Message);
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
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'number' is required.", result.Error.Message);

    }

    [Theory]
    [InlineData("abc")]
    [InlineData("45abc")]
    [InlineData("ab45")]
    [InlineData("4-5")]
    [InlineData("Denmark")]
    public void Create_WithNonNumericCountryCode_ReturnsFailureResult(string countryCode)
    {
        // Arrange
        var number = "12345678";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'countryCode' is not valid in this context.", result.Error.Message);

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
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'number' is not valid in this context.", result.Error.Message);

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
    public void Create_WithBothParametersNull_ReturnsMultipleErrors()
    {
        // Arrange
        string countryCode = null;
        string number = null;

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'countryCode' is required.", result.Error.Message);
        Assert.Contains("Value 'number' is required.", result.Error.Message);
    }

    [Fact]
    public void Create_WithBothParametersInvalid_ReturnsMultipleErrors()
    {
        // Arrange
        var countryCode = "abc";
        var number = "xyz";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'countryCode' is not valid in this context.", result.Error.Message);
        Assert.Contains("Value 'number' is not valid in this context.", result.Error.Message);
    }

    [Fact]
    public void Create_WithLongCountryCode_ReturnsFailureResult()
    {
        // Arrange
        var countryCode = "123456";
        var number = "12345678";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'countryCode' should not exceed 4.", result.Error.Message); 
    }

    [Fact]
    public void Create_WithLongNumber_ReturnsFailureResult()
    {
        // Arrange
        var countryCode = "1";
        var number = "12345678901234567890";

        // Act
        var result = PhoneNumber.Create(countryCode, number);

        // Assert
        Assert.True(result.Failure);
        Assert.Equal("MultipleErrors", result.Error.Code);
        Assert.Contains("Value 'number' should not exceed 15.", result.Error.Message);
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

    [Fact]
    public void GetEqualityComponents_SamePhoneNumberInstance_IsEqualToItself()
    {
        // Arrange
        var phoneNumber = PhoneNumber.Create("45", "12345678").Value;

        // Act & Assert
        Assert.Equal(phoneNumber, phoneNumber);
    }

    [Fact]
    public void GetEqualityComponents_PhoneNumberComparedToNull_IsNotEqual()
    {
        // Arrange
        var phoneNumber = PhoneNumber.Create("45", "12345678").Value;

        // Act & Assert
        Assert.NotEqual(phoneNumber, null);
    }

    [Fact]
    public void GetHashCode_TwoPhoneNumbersWithSameValues_HaveSameHashCode()
    {
        // Arrange
        var phoneNumber1 = PhoneNumber.Create("45", "12345678").Value;
        var phoneNumber2 = PhoneNumber.Create("45", "12345678").Value;

        // Act & Assert
        Assert.Equal(phoneNumber1.GetHashCode(), phoneNumber2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_TwoPhoneNumbersWithDifferentValues_HaveDifferentHashCodes()
    {
        // Arrange
        var phoneNumber1 = PhoneNumber.Create("45", "12345678").Value;
        var phoneNumber2 = PhoneNumber.Create("46", "87654321").Value;

        // Act & Assert
        Assert.NotEqual(phoneNumber1.GetHashCode(), phoneNumber2.GetHashCode());
    }
}
