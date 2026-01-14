using Helpers;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using Assert = Xunit.Assert;

namespace StoreManager.Domain.Tests.Chain;

[TestClass]
public class ChainEntityTests
{
    #region Create Tests

    [Fact]
    public void Create_WithValidName_ReturnsSuccessResult()
    {
        // Arrange
        var chainName = "Test Chain";

        // Act
        var result = ChainEntity.Create(chainName);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(chainName, result.Value.Name);
        Assert.NotNull(result.Value.Id);
    }

    [Fact]
    public void Create_WithValidName_SetsCreatedOnAndModifiedOn()
    {
        // Arrange
        var chainName = "Test Chain";
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = ChainEntity.Create(chainName);

        // Assert
        var afterCreation = DateTime.UtcNow;
        Assert.True(result.Success);
        Assert.True(result.Value.CreatedOn >= beforeCreation && result.Value.CreatedOn <= afterCreation);
        Assert.True(result.Value.ModifiedOn >= beforeCreation && result.Value.ModifiedOn <= afterCreation);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_WithEmptyName_ThrowsArgumentException(string name)
    {
        // Arrange
        string chainName = name;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ChainEntity.Create(chainName));
    }

    [Fact]
    public void Create_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        string? chainName = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => ChainEntity.Create(chainName));
    }

    #endregion

    #region AddStoreToChain Tests

    [Fact]
    public void AddStoreToChain_WithValidStore_AddsStoreToCollection()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value);

        // Act
        chain.AddStoreToChain(store);

        // Assert
        Assert.Single(chain.Stores);
        Assert.Contains(store.Value, chain.Stores);
    }

    [Fact]
    public void AddStoreToChain_WithMultipleStores_AddsAllStores()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var store1 = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value);
        var store2 = StoreEntity.Create(chain.Id, 2, "Test Store 2", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value);

        // Act
        chain.AddStoreToChain(store1);
        chain.AddStoreToChain(store2);

        // Assert
        Assert.Equal(2, chain.Stores.Count);
        Assert.Contains(store1.Value, chain.Stores);
        Assert.Contains(store2.Value, chain.Stores);
    }

    [Fact]
    public void AddStoreToChain_WithNullStore_ThrowsArgumentNullException()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chain.AddStoreToChain(null));
    }

    #endregion

    #region AddRangeStoresToChain Tests

    [Fact]
    public void AddRangeStoresToChain_WithValidStores_AddsAllStores()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var stores = new List<StoreEntity>
        {
            StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value),
            StoreEntity.Create(chain.Id, 2, "Test Store 2", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value),
            StoreEntity.Create(chain.Id, 3, "Test Store 3", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value)
        };

        // Act
        chain.AddRangeStoresToChain(stores);

        // Assert
        Assert.Equal(3, chain.Stores.Count);
        foreach (var store in stores)
        {
            Assert.Contains(store, chain.Stores);
        }
    }

    [Fact]
    public void AddRangeStoresToChain_WithEmptyList_DoesNotThrow()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var stores = new List<StoreEntity>();

        // Act
        chain.AddRangeStoresToChain(stores);

        // Assert
        Assert.Empty(chain.Stores);
    }

    [Fact]
    public void AddRangeStoresToChain_WithNullList_ThrowsArgumentNullException()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chain.AddRangeStoresToChain(null));
    }

    [Fact]
    public void AddRangeStoresToChain_AfterAddingIndividualStores_CombinesCollections()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var individualStore = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value);
        chain.AddStoreToChain(individualStore);

        var stores = new List<StoreEntity>
        {
            StoreEntity.Create(chain.Id, 2, "Test Store 2", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value),
            StoreEntity.Create(chain.Id, 3, "Test Store 3", Address.Create("123 Main St", "12345", "TestCity").Value, PhoneNumber.Create("+1", "1234567890").Value, Email.Create("test@example.com").Value, FullName.Create("John", "Doe").Value)
        };

        // Act
        chain.AddRangeStoresToChain(stores);

        // Assert
        Assert.Equal(3, chain.Stores.Count);
        Assert.Contains(individualStore.Value, chain.Stores);
    }

    #endregion

    #region UpdateChainDetails Tests

    [Fact]
    public void UpdateChainDetails_WithValidName_UpdatesName()
    {
        // Arrange
        var chain = ChainEntity.Create("Original Name").Value;
        var newName = "Updated Name";

        // Act
        chain.UpdateChainDetails(newName);

        // Assert
        Assert.Equal(newName, chain.Name);
    }

    [Fact]
    public void UpdateChainDetails_WithValidName_UpdatesModifiedOn()
    {
        // Arrange
        var chain = ChainEntity.Create("Original Name").Value;
        var originalModifiedOn = chain.ModifiedOn;
        Thread.Sleep(10); // Ensure time difference
        var beforeUpdate = DateTime.UtcNow;

        // Act
        chain.UpdateChainDetails("Updated Name");

        // Assert
        var afterUpdate = DateTime.UtcNow;
        Assert.True(chain.ModifiedOn > originalModifiedOn);
        Assert.True(chain.ModifiedOn >= beforeUpdate && chain.ModifiedOn <= afterUpdate);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void UpdateChainDetails_WithEmptyName_ThrowsArgumentException(string name)
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => chain.UpdateChainDetails(name));
    }

    [Fact]
    public void UpdateChainDetails_WithNullName_ThrowsArgumentNullException()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => chain.UpdateChainDetails(null));
    }

    #endregion

    #region Property Tests

    [Fact]
    public void Stores_ReturnsReadOnlyList()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;

        // Act
        var stores = chain.Stores;

        // Assert
        Assert.NotNull(stores);
        Assert.IsAssignableFrom<IReadOnlyList<StoreEntity>>(stores);
    }

    [Fact]
    public void Stores_InitializedAsEmpty()
    {
        // Arrange & Act
        var chain = ChainEntity.Create("Test Chain").Value;

        // Assert
        Assert.NotNull(chain.Stores);
        Assert.Empty(chain.Stores);
    }

    #endregion
}
