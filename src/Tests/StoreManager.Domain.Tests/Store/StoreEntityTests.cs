using Helpers;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using System.IO;
using Assert = Xunit.Assert;

namespace StoreManager.Domain.Tests.Store;

[TestClass]
public class StoreEntityTests
{
    #region Create Tests

    [Fact]
    public void Create_WithValidParameters_ReturnsSuccessResult()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var number = 1;
        var name = "Test Store";
        var address = Address.Create("123 Main St", "12345", "TestCity").Value;
        var phoneNumber = PhoneNumber.Create("+1", "1234567890").Value;
        var email = Email.Create("test@example.com").Value;
        var storeOwner = FullName.Create("John", "Doe").Value;

        // Act
        var result = StoreEntity.Create(chainId, number, name, address, phoneNumber, email, storeOwner);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(chainId, result.Value.ChainId);
        Assert.Equal(number, result.Value.Number);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(address, result.Value.Address);
        Assert.Equal(phoneNumber, result.Value.PhoneNumber);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(storeOwner, result.Value.StoreOwner);
        Assert.NotNull(result.Value.Id);
    }

    [Fact]
    public void Create_WithNullChainId_ReturnsSuccessResult()
    {
        // Arrange
        ChainId? chainId = null;
        int number = 1;
        string name = "Test Store";
        var address = Address.Create("123 Main St", "12345", "TestCity").Value;
        var phoneNumber = PhoneNumber.Create("+1", "1234567890").Value;
        var email = Email.Create("test@example.com").Value;
        var storeOwner = FullName.Create("John", "Doe").Value;

        // Act
        var result = StoreEntity.Create(chainId, number, name, address, phoneNumber, email, storeOwner);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Null(result.Value.ChainId);
    }

    [Fact]
    public void Create_WithValidParameters_SetsCreatedOnAndModifiedOn()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var number = 1;
        var name = "Test Store";
        var address = Address.Create("123 Main St", "12345", "TestCity").Value;
        var phoneNumber = PhoneNumber.Create("+1", "1234567890").Value;
        var email = Email.Create("test@example.com").Value;
        var storeOwner = FullName.Create("John", "Doe").Value;
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = StoreEntity.Create(chainId, number, name, address, phoneNumber, email, storeOwner);

        // Assert
        var afterCreation = DateTime.UtcNow;
        Assert.True(result.Success);
        Assert.True(result.Value.CreatedOn >= beforeCreation && result.Value.CreatedOn <= afterCreation);
        Assert.True(result.Value.ModifiedOn >= beforeCreation && result.Value.ModifiedOn <= afterCreation);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData(null, " ")]
    public void Create_WithInvalidParameters_ThrowsArgumentException(int testNumber, string testName)
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var number = testNumber;
        var name = testName;
        var address = Address.Create("123 Main St", "12345", "TestCity").Value;
        var phoneNumber = PhoneNumber.Create("+1", "1234567890").Value;
        var email = Email.Create("test@example.com").Value;
        var storeOwner = FullName.Create("John", "Doe").Value;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => StoreEntity.Create(chainId, number, name, address, phoneNumber, email, storeOwner));
    }

    [Fact]
    public void Create_WithInvalidParameters_ThrowsArgumentNullException()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        int number = 1;
        string name = null;
        var address = Address.Create("123 Main St", "12345", "TestCity").Value;
        var phoneNumber = PhoneNumber.Create("+1", "1234567890").Value;
        var email = Email.Create("test@example.com").Value;
        var storeOwner = FullName.Create("John", "Doe").Value;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => StoreEntity.Create(chainId, number, name, address, phoneNumber, email, storeOwner));
    }

    #endregion

    #region UpdateStore Tests

    [Fact]
    public void UpdateStore_WithValidParameters_UpdatesAllProperties()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var store = StoreEntity.Create(
            chainId, 
            1, 
            "Original Store",
            Address.Create("123 Main St", "12345", "OldCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Jane", "Smith").Value).Value;

        var newChainId = ChainId.Create().Value;
        var newNumber = 2;
        var newName = "Updated Store";
        var newAddress = Address.Create("456 Oak Ave", "67890", "NewCity").Value;
        var newPhoneNumber = PhoneNumber.Create("+44", "9876543210").Value;
        var newEmail = Email.Create("new@example.com").Value;
        var newStoreOwner = FullName.Create("John", "Doe").Value;
        var beforeUpdate = DateTime.UtcNow;

        // Act
        store.UpdateStore(newChainId, newNumber, newName, newAddress, newPhoneNumber, newEmail, newStoreOwner);

        // Assert
        var afterUpdate = DateTime.UtcNow;
        Assert.Equal(newChainId, store.ChainId);
        Assert.Equal(newNumber, store.Number);
        Assert.Equal(newName, store.Name);
        Assert.Equal(newAddress, store.Address);
        Assert.Equal(newPhoneNumber, store.PhoneNumber);
        Assert.Equal(newEmail, store.Email);
        Assert.Equal(newStoreOwner, store.StoreOwner);
        Assert.True(store.ModifiedOn >= beforeUpdate && store.ModifiedOn <= afterUpdate);
    }

    [Fact]
    public void UpdateStore_WithNullChainId_UpdatesChainIdToNull()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var store = StoreEntity.Create(
            chainId, 
            1, 
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("test@example.com").Value,
            FullName.Create("Jane", "Smith").Value).Value;

        var newAddress = Address.Create("456 Oak Ave", "67890", "NewCity").Value;
        var newPhoneNumber = PhoneNumber.Create("+44", "9876543210").Value;
        var newEmail = Email.Create("new@example.com").Value;
        var newStoreOwner = FullName.Create("John", "Doe").Value;

        // Act
        store.UpdateStore(null, 2, "Updated Store", newAddress, newPhoneNumber, newEmail, newStoreOwner);

        // Assert
        Assert.Null(store.ChainId);
    }

    [Fact]
    public void UpdateStore_WithValidChainId_UpdatesChainId()
    {
        // Arrange
        var store = StoreEntity.Create(
            null, 
            1, 
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("test@example.com").Value,
            FullName.Create("Jane", "Smith").Value).Value;

        var newChainId = ChainId.Create().Value;
        var newAddress = Address.Create("456 Oak Ave", "67890", "NewCity").Value;
        var newPhoneNumber = PhoneNumber.Create("+44", "9876543210").Value;
        var newEmail = Email.Create("new@example.com").Value;
        var newStoreOwner = FullName.Create("John", "Doe").Value;

        // Act
        store.UpdateStore(newChainId, 2, "Updated Store", newAddress, newPhoneNumber, newEmail, newStoreOwner);

        // Assert
        Assert.Equal(newChainId, store.ChainId);
    }

    [Fact]
    public void UpdateStore_UpdatesModifiedOn_ButNotCreatedOn()
    {
        // Arrange
        var store = StoreEntity.Create(
            null, 
            1, 
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("test@example.com").Value,
            FullName.Create("Jane", "Smith").Value).Value;

        var originalCreatedOn = store.CreatedOn;
        var originalModifiedOn = store.ModifiedOn;
        Thread.Sleep(10); // Ensure time difference
        var beforeUpdate = DateTime.UtcNow;

        var newAddress = Address.Create("456 Oak Ave", "67890", "NewCity").Value;
        var newPhoneNumber = PhoneNumber.Create("+44", "9876543210").Value;
        var newEmail = Email.Create("new@example.com").Value;
        var newStoreOwner = FullName.Create("John", "Doe").Value;

        // Act
        store.UpdateStore(null, 2, "Updated Store", newAddress, newPhoneNumber, newEmail, newStoreOwner);

        // Assert
        var afterUpdate = DateTime.UtcNow;
        Assert.Equal(originalCreatedOn, store.CreatedOn);
        Assert.NotEqual(originalModifiedOn, store.ModifiedOn);
        Assert.True(store.ModifiedOn >= beforeUpdate && store.ModifiedOn <= afterUpdate);
    }

    #endregion

    #region Properties Tests

    [Fact]
    public void Properties_AreSetCorrectly_AfterCreation()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var number = 5;
        var name = "Test Store";
        var address = Address.Create("123 Main St", "12345", "TestCity").Value;
        var phoneNumber = PhoneNumber.Create("+1", "1234567890").Value;
        var email = Email.Create("test@example.com").Value;
        var storeOwner = FullName.Create("John", "Doe").Value;

        // Act
        var result = StoreEntity.Create(chainId, number, name, address, phoneNumber, email, storeOwner);

        // Assert
        var store = result.Value;
        Assert.Equal(chainId, store.ChainId);
        Assert.Equal(number, store.Number);
        Assert.Equal(name, store.Name);
        Assert.Equal(address.Street, store.Address.Street);
        Assert.Equal(address.PostalCode, store.Address.PostalCode);
        Assert.Equal(address.City, store.Address.City);
        Assert.Equal(phoneNumber.CountryCode, store.PhoneNumber.CountryCode);
        Assert.Equal(phoneNumber.Number, store.PhoneNumber.Number);
        Assert.Equal(email.Value, store.Email.Value);
        Assert.Equal(storeOwner.FirstName, store.StoreOwner.FirstName);
        Assert.Equal(storeOwner.LastName, store.StoreOwner.LastName);
    }

    #endregion
}
