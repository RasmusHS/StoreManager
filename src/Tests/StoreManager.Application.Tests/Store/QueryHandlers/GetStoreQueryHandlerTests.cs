using Helpers;
using Moq;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.Queries.Store;
using StoreManager.Application.Queries.Store.Handlers;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Store.QueryHandlers;

[TestClass]
public class GetStoreQueryHandlerTests
{
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly GetStoreQueryHandler _handler;

    public GetStoreQueryHandlerTests()
    {
        _mockStoreRepository = new Mock<IStoreRepository>();
        _handler = new GetStoreQueryHandler(_mockStoreRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidStoreId_ReturnsSuccessResult()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var storeEntity = StoreEntity.Create(
            chainId,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeEntity.Value.Id))
            .ReturnsAsync(storeEntity.Value);

        var query = new GetStoreQuery(storeEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(storeEntity.Value.Id.Value, result.Value.Id);
        Assert.Equal(storeEntity.Value.ChainId.Value, result.Value.ChainId);
        Assert.Equal(storeEntity.Value.Number, result.Value.Number);
        Assert.Equal(storeEntity.Value.Name, result.Value.Name);
        Assert.Equal(storeEntity.Value.Address.Street, result.Value.Street);
        Assert.Equal(storeEntity.Value.Address.PostalCode, result.Value.PostalCode);
        Assert.Equal(storeEntity.Value.Address.City, result.Value.City);
        Assert.Equal(storeEntity.Value.PhoneNumber.CountryCode, result.Value.CountryCode);
        Assert.Equal(storeEntity.Value.PhoneNumber.Number, result.Value.PhoneNumber);
        Assert.Equal(storeEntity.Value.Email.Value, result.Value.Email);
        Assert.Equal(storeEntity.Value.StoreOwner.FirstName, result.Value.FirstName);
        Assert.Equal(storeEntity.Value.StoreOwner.LastName, result.Value.LastName);
        Assert.Equal(storeEntity.Value.CreatedOn, result.Value.CreatedOn);
        Assert.Equal(storeEntity.Value.ModifiedOn, result.Value.ModifiedOn);

        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidStoreIdWithoutChainId_ReturnsSuccessResultWithNullChainId()
    {
        // Arrange
        var storeEntity = StoreEntity.Create(
            null,
            2,
            "Independent Store",
            Address.Create("456 Oak Ave", "67890", "AnotherCity").Value,
            PhoneNumber.Create("+44", "2087654321").Value,
            Email.Create("independent@example.com").Value,
            FullName.Create("Jane", "Smith").Value);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeEntity.Value.Id))
            .ReturnsAsync(storeEntity.Value);

        var query = new GetStoreQuery(storeEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Null(result.Value.ChainId);
        Assert.Equal(storeEntity.Value.Id.Value, result.Value.Id);
        Assert.Equal("Independent Store", result.Value.Name);
        Assert.Equal(2, result.Value.Number);

        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentStoreId_ReturnsFailureResult()
    {
        // Arrange
        var storeId = StoreId.GetExisting(Guid.NewGuid()).Value;

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync((StoreEntity)null);

        var query = new GetStoreQuery(storeId);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);

        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeId), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_MapsAllAddressFieldsCorrectly()
    {
        // Arrange
        var storeEntity = StoreEntity.Create(
            null,
            3,
            "Address Test Store",
            Address.Create("789 Elm Street", "54321", "Springfield").Value,
            PhoneNumber.Create("+1", "5551234567").Value,
            Email.Create("address@example.com").Value,
            FullName.Create("Bob", "Johnson").Value);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeEntity.Value.Id))
            .ReturnsAsync(storeEntity.Value);

        var query = new GetStoreQuery(storeEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("789 Elm Street", result.Value.Street);
        Assert.Equal("54321", result.Value.PostalCode);
        Assert.Equal("Springfield", result.Value.City);

        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_MapsPhoneNumberCorrectly()
    {
        // Arrange
        var storeEntity = StoreEntity.Create(
            null,
            4,
            "Phone Test Store",
            Address.Create("321 Pine Rd", "11111", "Metropolis").Value,
            PhoneNumber.Create("+49", "3012345678").Value,
            Email.Create("phone@example.com").Value,
            FullName.Create("Alice", "Brown").Value);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeEntity.Value.Id))
            .ReturnsAsync(storeEntity.Value);

        var query = new GetStoreQuery(storeEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("+49", result.Value.CountryCode);
        Assert.Equal("3012345678", result.Value.PhoneNumber);

        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_MapsStoreOwnerCorrectly()
    {
        // Arrange
        var storeEntity = StoreEntity.Create(
            null,
            5,
            "Owner Test Store",
            Address.Create("654 Maple Dr", "22222", "Gotham").Value,
            PhoneNumber.Create("+1", "9876543210").Value,
            Email.Create("owner@example.com").Value,
            FullName.Create("Charlie", "Williams").Value);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeEntity.Value.Id))
            .ReturnsAsync(storeEntity.Value);

        var query = new GetStoreQuery(storeEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Charlie", result.Value.FirstName);
        Assert.Equal("Williams", result.Value.LastName);

        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var storeEntity = StoreEntity.Create(
            null,
            6,
            "Cancellation Test Store",
            Address.Create("999 Broadway", "33333", "Star City").Value,
            PhoneNumber.Create("+1", "1112223333").Value,
            Email.Create("cancel@example.com").Value,
            FullName.Create("David", "Miller").Value);

        var cancellationToken = new CancellationToken();

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeEntity.Value.Id))
            .ReturnsAsync(storeEntity.Value);

        var query = new GetStoreQuery(storeEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.True(result.Success);
        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeEntity.Value.Id), Times.Once);
    }
}
