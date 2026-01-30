using Helpers;
using Moq;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.Queries.Chain;
using StoreManager.Application.Queries.Chain.Handlers;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Chain.QueryHandlers;

[TestClass]
public class GetChainAndStoresQueryHandlerTests
{
    private readonly Mock<IChainRepository> _mockChainRepository;
    private readonly GetChainAndStoresQueryHandler _handler;

    public GetChainAndStoresQueryHandlerTests()
    {
        _mockChainRepository = new Mock<IChainRepository>();
        _handler = new GetChainAndStoresQueryHandler(_mockChainRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidChainIdAndStores_ReturnsSuccessResult()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Test Chain").Value;
        var store1 = StoreEntity.Create(
            chainEntity.Id,
            1,
            "Store 1",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store1@example.com").Value,
            FullName.Create("John", "Doe").Value).Value;

        var store2 = StoreEntity.Create(
            chainEntity.Id,
            2,
            "Store 2",
            Address.Create("456 Oak Ave", "67890", "OtherCity").Value,
            PhoneNumber.Create("+1", "9876543210").Value,
            Email.Create("store2@example.com").Value,
            FullName.Create("Jane", "Smith").Value).Value;

        var storeList = new List<StoreEntity> { store1, store2 };

        chainEntity.AddRangeStoresToChain(storeList);

        _mockChainRepository
            .Setup(r => r.GetByIdIncludeStoresAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        var query = new GetChainAndStoresQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(chainEntity.Id.Value, result.Value.Id);
        Assert.Equal(chainEntity.Name, result.Value.Name);
        Assert.Equal(chainEntity.CreatedOn, result.Value.CreatedOn);
        Assert.Equal(chainEntity.ModifiedOn, result.Value.ModifiedOn);
        Assert.NotNull(result.Value.Stores);
        Assert.Equal(2, result.Value.Stores.Count);

        // Verify first store
        var storeDto1 = result.Value.Stores.FirstOrDefault(s => s.Number == 1);
        Assert.NotNull(storeDto1);
        Assert.Equal(store1.Id.Value, storeDto1.Id);
        Assert.Equal(store1.ChainId!.Value, storeDto1.ChainId);
        Assert.Equal(store1.Number, storeDto1.Number);
        Assert.Equal(store1.Name, storeDto1.Name);
        Assert.Equal(store1.Address.Street, storeDto1.Street);
        Assert.Equal(store1.Address.PostalCode, storeDto1.PostalCode);
        Assert.Equal(store1.Address.City, storeDto1.City);
        Assert.Equal(store1.PhoneNumber.CountryCode, storeDto1.CountryCode);
        Assert.Equal(store1.PhoneNumber.Number, storeDto1.PhoneNumber);
        Assert.Equal(store1.Email.Value, storeDto1.Email);
        Assert.Equal(store1.StoreOwner.FirstName, storeDto1.FirstName);
        Assert.Equal(store1.StoreOwner.LastName, storeDto1.LastName);
        Assert.Equal(store1.CreatedOn, storeDto1.CreatedOn);
        Assert.Equal(store1.ModifiedOn, storeDto1.ModifiedOn);

        // Verify second store
        var storeDto2 = result.Value.Stores.FirstOrDefault(s => s.Number == 2);
        Assert.NotNull(storeDto2);
        Assert.Equal(store2.Id.Value, storeDto2.Id);
        Assert.Equal(store2.Name, storeDto2.Name);

        _mockChainRepository.Verify(r => r.GetByIdIncludeStoresAsync(chainEntity.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentChainId_ReturnsFailureResult()
    {
        // Arrange
        var chainId = ChainId.GetExisting(Guid.NewGuid()).Value;

        _mockChainRepository
            .Setup(r => r.GetByIdIncludeStoresAsync(chainId))
            .ReturnsAsync((ChainEntity)null);

        var query = new GetChainAndStoresQuery(chainId);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal($"Could not find entity with ID {chainId}.", result.Error.Message);

        _mockChainRepository.Verify(r => r.GetByIdIncludeStoresAsync(chainId), Times.Once);
    }

    [Fact]
    public async Task Handle_WithChainHavingNoStores_ReturnsSuccessResult()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Empty Chain").Value;

        _mockChainRepository
            .Setup(r => r.GetByIdIncludeStoresAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        var query = new GetChainAndStoresQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value.Stores!);
        Assert.Equal(chainEntity.Id.Value, result.Value.Id);
        Assert.Equal(chainEntity.Name, result.Value.Name);
        Assert.Equal(chainEntity.CreatedOn, result.Value.CreatedOn);
        Assert.Equal(chainEntity.ModifiedOn, result.Value.ModifiedOn);

        _mockChainRepository.Verify(r => r.GetByIdIncludeStoresAsync(chainEntity.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesCancellationTokenCorrectly()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(
            chainEntity.Id,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("test@example.com").Value,
            FullName.Create("John", "Doe").Value).Value;

        var storeList = new List<StoreEntity> { store };

        chainEntity.AddRangeStoresToChain(storeList);

        var cancellationToken = new CancellationToken();

        _mockChainRepository
            .Setup(r => r.GetByIdIncludeStoresAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        var query = new GetChainAndStoresQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Stores!);
        Assert.Equal(store.Id.Value, result.Value.Stores![0].Id);
        Assert.Equal(store.Name, result.Value.Stores![0].Name);
        Assert.Equal(store.Address.Street, result.Value.Stores![0].Street);
        Assert.Equal(store.Address.PostalCode, result.Value.Stores![0].PostalCode);
        Assert.Equal(store.Address.City, result.Value.Stores![0].City);
        Assert.Equal(store.PhoneNumber.Number, result.Value.Stores![0].PhoneNumber);
        Assert.Equal(store.Email.Value, result.Value.Stores![0].Email);
        Assert.Equal(store.StoreOwner.FirstName, result.Value.Stores![0].FirstName);
        Assert.Equal(store.StoreOwner.LastName, result.Value.Stores![0].LastName);

        _mockChainRepository.Verify(r => r.GetByIdIncludeStoresAsync(chainEntity.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithSingleStore_ReturnsSuccessResultWithOneStore()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Single Store Chain").Value;
        var store = StoreEntity.Create(
            chainEntity.Id,
            1,
            "Only Store",
            Address.Create("789 Elm St", "11111", "CityName").Value,
            PhoneNumber.Create("+44", "2012345678").Value,
            Email.Create("only@example.com").Value,
            FullName.Create("Alice", "Johnson").Value).Value;

        var storeList = new List<StoreEntity> { store };

        chainEntity.AddRangeStoresToChain(storeList);

        _mockChainRepository
            .Setup(r => r.GetByIdIncludeStoresAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        var query = new GetChainAndStoresQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Stores!);
        Assert.Equal(store.Id.Value, result.Value.Stores![0].Id);
        Assert.Equal(store.Name, result.Value.Stores![0].Name);
        Assert.Equal(store.Address.Street, result.Value.Stores![0].Street);
        Assert.Equal(store.Address.PostalCode, result.Value.Stores![0].PostalCode);
        Assert.Equal(store.Address.City, result.Value.Stores![0].City);
        Assert.Equal(store.PhoneNumber.Number, result.Value.Stores![0].PhoneNumber);
        Assert.Equal(store.Email.Value, result.Value.Stores![0].Email);
        Assert.Equal(store.StoreOwner.FirstName, result.Value.Stores![0].FirstName);
        Assert.Equal(store.StoreOwner.LastName, result.Value.Stores![0].LastName);

        _mockChainRepository.Verify(r => r.GetByIdIncludeStoresAsync(chainEntity.Id), Times.Once);
    }
}
