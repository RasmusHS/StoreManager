using Assert = Xunit.Assert;
using Helpers;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Domain.Tests.Common.ValueObjects;

[TestClass]
public class EmailTests
{
    [Fact]
    public void Create_WithValidEmail_ShouldSucceed()
    {
        // Arrange
        var emailString = "test@example.com";

        // Act
        var result = Email.Create(emailString);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(emailString, result.Value.Value);
    }

    [Fact]
    public void Create_WithValidEmailAndWhitespace_ShouldTrimAndSucceed()
    {
        // Arrange
        var emailString = "  test@example.com  ";

        // Act
        var result = Email.Create(emailString);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("test@example.com", result.Value.Value);
    }

    [Fact]
    public void Create_WithNullAndNotRequired_ShouldFail()
    {
        // Arrange
        string email = null;

        // Act
        var result = Email.Create(email, isRequired: false);

        // Assert
        Assert.True(result.Failure);
        Assert.Contains<Error>(Errors.General.ValueIsRequired(nameof(email)), (IEnumerable<Error>)result.Error);
    }

    [Fact]
    public void Create_WithEmptyStringAndRequired_ShouldFail()
    {
        // Arrange
        var email = string.Empty;

        // Act
        var result = Email.Create(email, isRequired: true);

        // Assert
        Assert.True(result.Failure);
        Assert.Contains<Error>(Errors.General.ValueIsRequired(nameof(email)), (IEnumerable<Error>)result.Error);
    }

    [Fact]
    public void Create_WithNullAndRequired_ShouldFail()
    {
        // Arrange
        string email = null;

        // Act
        var result = Email.Create(email, isRequired: true);

        // Assert
        Assert.True(result.Failure);
        Assert.Contains<Error>(Errors.General.ValueIsRequired(nameof(email)), (IEnumerable<Error>)result.Error);
    }

    [Fact]
    public void Create_WithEmailTooLong_ShouldFail()
    {
        // Arrange
        var email = new string('a', 100) + "@test.com";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.Failure);
        Assert.Contains<Error>(Errors.General.ValueTooLarge(nameof(email), 100), (IEnumerable<Error>)result.Error);
    }

    [Fact]
    public void Create_WithInvalidEmailFormat_ShouldFail()
    {
        // Arrange
        var email = "notanemail";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.Failure);
        Assert.Contains<Error>(Errors.General.UnexpectedValue(nameof(email)), (IEnumerable<Error>)result.Error);
    }

    [Fact]
    public void Create_WithMissingAtSign_ShouldFail()
    {
        // Arrange
        var email = "testexample.com";

        // Act
        var result = Email.Create(email);

        // Assert
        Assert.True(result.Failure);
        Assert.Contains<Error>(Errors.General.UnexpectedValue(nameof(email)), (IEnumerable<Error>)result.Error);
    }

    [Fact]
    public void ImplicitOperator_ShouldConvertEmailToString()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value;

        // Act
        string emailString = email;

        // Assert
        Assert.Equal("test@example.com", emailString);
    }

    [Fact]
    public void ExplicitOperator_ShouldConvertStringToEmail()
    {
        // Arrange
        var emailString = "test@example.com";

        // Act
        var email = (Email)emailString;

        // Assert
        Assert.Equal(emailString, email.Value);
    }

    [Fact]
    public void GetEqualityComponents_ShouldReturnValue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com").Value;
        var email2 = Email.Create("test@example.com").Value;
        var email3 = Email.Create("different@example.com").Value;

        // Assert
        Assert.Equal(email1, email2);
        Assert.NotEqual(email1, email3);
        Assert.NotEqual(email2, email3);
    }
}
