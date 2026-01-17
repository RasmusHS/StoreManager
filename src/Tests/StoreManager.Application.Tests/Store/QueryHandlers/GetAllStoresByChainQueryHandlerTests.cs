using Helpers;
using Moq;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.Queries.Store;
using StoreManager.Application.Queries.Store.Handlers;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Store.QueryHandlers;

[TestClass]
public class GetAllStoresByChainQueryHandlerTests
{
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly Mock<IChainRepository> _mockChainRepository;
    private readonly GetAllStoresByChainQueryHandler _handler;

    public GetAllStoresByChainQueryHandlerTests()
    {
        _mockStoreRepository = new Mock<IStoreRepository>();
        _mockChainRepository = new Mock<IChainRepository>();
        _handler = new GetAllStoresByChainQueryHandler(_mockStoreRepository.Object, _mockChainRepository.Object);
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

        var stores = new List<StoreEntity> { store1, store2 };

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.GetAllByChainIdAsync(chainEntity.Id))
            .ReturnsAsync(stores);

        var query = new GetAllStoresByChainQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Data);
        Assert.Equal(2, result.Value.Data.Count());

        // Verify first store
        var storeDto1 = result.Value.Data.FirstOrDefault(s => s.Number == 1);
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
        var storeDto2 = result.Value.Data.FirstOrDefault(s => s.Number == 2);
        Assert.NotNull(storeDto2);
        Assert.Equal(store2.Id.Value, storeDto2.Id);
        Assert.Equal(store2.ChainId!.Value, storeDto2.ChainId);
        Assert.Equal(store2.Number, storeDto2.Number);
        Assert.Equal(store2.Name, storeDto2.Name);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Id), Times.Once);
        _mockStoreRepository.Verify(r => r.GetAllByChainIdAsync(chainEntity.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentChainId_ReturnsFailureResult()
    {
        // Arrange
        var chainId = ChainId.GetExisting(Guid.NewGuid()).Value;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync((ChainEntity)null);

        var query = new GetAllStoresByChainQuery(chainId);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockStoreRepository.Verify(r => r.GetAllByChainIdAsync(It.IsAny<ChainId>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithChainHavingNoStores_ReturnsFailureResult()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Empty Chain").Value;
        var emptyStoreList = new List<StoreEntity>();

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.GetAllByChainIdAsync(chainEntity.Id))
            .ReturnsAsync(emptyStoreList);

        var query = new GetAllStoresByChainQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Contains("has no stores", result.Error.Message.ToLower());

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Id), Times.Once);
        _mockStoreRepository.Verify(r => r.GetAllByChainIdAsync(chainEntity.Id), Times.Once);
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
            Address.Create("789 Pine Rd", "11111", "SingleCity").Value,
            PhoneNumber.Create("+44", "2012345678").Value,
            Email.Create("onlystore@example.com").Value,
            FullName.Create("Alice", "Johnson").Value).Value;

        var stores = new List<StoreEntity> { store };

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.GetAllByChainIdAsync(chainEntity.Id))
            .ReturnsAsync(stores);

        var query = new GetAllStoresByChainQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Data);
        Assert.Single(result.Value.Data);

        var storeDto = result.Value.Data.First();
        Assert.Equal(store.Id.Value, storeDto.Id);
        Assert.Equal(store.ChainId!.Value, storeDto.ChainId);
        Assert.Equal(store.Number, storeDto.Number);
        Assert.Equal(store.Name, storeDto.Name);
        Assert.Equal(store.Address.Street, storeDto.Street);
        Assert.Equal(store.Address.PostalCode, storeDto.PostalCode);
        Assert.Equal(store.Address.City, storeDto.City);
        Assert.Equal(store.PhoneNumber.CountryCode, storeDto.CountryCode);
        Assert.Equal(store.PhoneNumber.Number, storeDto.PhoneNumber);
        Assert.Equal(store.Email.Value, storeDto.Email);
        Assert.Equal(store.StoreOwner.FirstName, storeDto.FirstName);
        Assert.Equal(store.StoreOwner.LastName, storeDto.LastName);
        Assert.Equal(store.CreatedOn, storeDto.CreatedOn);
        Assert.Equal(store.ModifiedOn, storeDto.ModifiedOn);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Id), Times.Once);
        _mockStoreRepository.Verify(r => r.GetAllByChainIdAsync(chainEntity.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithMultipleStores_ReturnsAllStoresInCorrectOrder()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Multi Store Chain").Value;
        var store1 = StoreEntity.Create(
            chainEntity.Id,
            5,
            "Store Five",
            Address.Create("100 First Ave", "22222", "CityA").Value,
            PhoneNumber.Create("+1", "5551234567").Value,
            Email.Create("store5@example.com").Value,
            FullName.Create("Bob", "Williams").Value).Value;

        var store2 = StoreEntity.Create(
            chainEntity.Id,
            3,
            "Store Three",
            Address.Create("200 Second Ave", "33333", "CityB").Value,
            PhoneNumber.Create("+1", "5557654321").Value,
            Email.Create("store3@example.com").Value,
            FullName.Create("Carol", "Brown").Value).Value;

        var store3 = StoreEntity.Create(
            chainEntity.Id,
            1,
            "Store One",
            Address.Create("300 Third Ave", "44444", "CityC").Value,
            PhoneNumber.Create("+1", "5559876543").Value,
            Email.Create("store1@example.com").Value,
            FullName.Create("David", "Miller").Value).Value;

        var stores = new List<StoreEntity> { store1, store2, store3 };

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.GetAllByChainIdAsync(chainEntity.Id))
            .ReturnsAsync(stores);

        var query = new GetAllStoresByChainQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value.Data);
        Assert.Equal(3, result.Value.Data.Count());

        // Verify all stores are present
        Assert.Contains(result.Value.Data, s => s.Number == 5 && s.Name == "Store Five");
        Assert.Contains(result.Value.Data, s => s.Number == 3 && s.Name == "Store Three");
        Assert.Contains(result.Value.Data, s => s.Number == 1 && s.Name == "Store One");

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Id), Times.Once);
        _mockStoreRepository.Verify(r => r.GetAllByChainIdAsync(chainEntity.Id), Times.Once);
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
            FullName.Create("Test", "User").Value).Value;

        var stores = new List<StoreEntity> { store };
        var cancellationToken = new CancellationToken();

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Id))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.GetAllByChainIdAsync(chainEntity.Id))
            .ReturnsAsync(stores);

        var query = new GetAllStoresByChainQuery(chainEntity.Id);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.Data);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Id), Times.Once);
        _mockStoreRepository.Verify(r => r.GetAllByChainIdAsync(chainEntity.Id), Times.Once);
    }
}
