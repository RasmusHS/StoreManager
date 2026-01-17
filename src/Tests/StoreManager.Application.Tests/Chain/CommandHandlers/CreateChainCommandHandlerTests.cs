using Helpers;
using Moq;
using StoreManager.Application.Commands.Chain;
using StoreManager.Application.Commands.Chain.Handlers;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Common.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Chain.CommandHandlers;

[TestClass]
public class CreateChainCommandHandlerTests
{
    private readonly Mock<IChainRepository> _mockChainRepository;
    private readonly CreateChainCommandHandler _handler;

    public CreateChainCommandHandlerTests()
    {
        _mockChainRepository = new Mock<IChainRepository>();
        _handler = new CreateChainCommandHandler(_mockChainRepository.Object);
    }

    #region Create Chain Without Stores Tests

    [Fact]
    public async Task Handle_WithValidChainNameAndNoStores_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateChainCommand("Test Chain", null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Chain", result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Null(result.Value.Stores);
        _mockChainRepository.Verify(r => r.AddAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidChainNameAndEmptyStoresList_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateChainCommand("Test Chain", new List<CreateStoreCommand>());

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Chain", result.Value.Name);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.Null(result.Value.Stores);
        _mockChainRepository.Verify(r => r.AddAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidChainNameAndNoStores_SetsCreatedOnAndModifiedOn()
    {
        // Arrange
        var command = new CreateChainCommand("Test Chain", null);
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await _handler.Handle(command);

        // Assert
        var afterCreation = DateTime.UtcNow;
        Assert.True(result.Success);
        Assert.True(result.Value.CreatedOn >= beforeCreation && result.Value.CreatedOn <= afterCreation);
        Assert.True(result.Value.ModifiedOn >= beforeCreation && result.Value.ModifiedOn <= afterCreation);
    }

    #endregion

    #region Create Chain With Stores Tests

    [Fact]
    public async Task Handle_WithValidChainAndSingleStore_ReturnsSuccessResult()
    {
        // Arrange
        var stores = new List<CreateStoreCommand>
        {
            new CreateStoreCommand(
                null,
                1,
                "Store 1",
                Address.Create("123 Main St", "12345", "TestCity").Value,
                PhoneNumber.Create("+1", "1234567890").Value,
                Email.Create("store1@example.com").Value,
                FullName.Create("John", "Doe").Value)
        };
        var command = new CreateChainCommand("Test Chain", stores);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Chain", result.Value.Name);
        Assert.NotNull(result.Value.Stores);
        Assert.Single(result.Value.Stores);
        Assert.Equal("Store 1", result.Value.Stores[0].Name);
        Assert.Equal(1, result.Value.Stores[0].Number);
        Assert.Equal("store1@example.com", result.Value.Stores[0].Email);
        _mockChainRepository.Verify(r => r.AddAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidChainAndMultipleStores_ReturnsSuccessResultWithAllStores()
    {
        // Arrange
        var stores = new List<CreateStoreCommand>
        {
            new CreateStoreCommand(
                null,
                1,
                "Store 1",
                Address.Create("123 Main St", "12345", "TestCity").Value,
                PhoneNumber.Create("+1", "1234567890").Value,
                Email.Create("store1@example.com").Value,
                FullName.Create("John", "Doe").Value),
            new CreateStoreCommand(
                null,
                2,
                "Store 2",
                Address.Create("456 Oak Ave", "67890", "AnotherCity").Value,
                PhoneNumber.Create("+1", "0987654321").Value,
                Email.Create("store2@example.com").Value,
                FullName.Create("Jane", "Smith").Value),
            new CreateStoreCommand(
                null,
                3,
                "Store 3",
                Address.Create("789 Elm St", "11111", "ThirdCity").Value,
                PhoneNumber.Create("+1", "5555555555").Value,
                Email.Create("store3@example.com").Value,
                FullName.Create("Bob", "Johnson").Value)
        };
        var command = new CreateChainCommand("Test Chain", stores);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Chain", result.Value.Name);
        Assert.NotNull(result.Value.Stores);
        Assert.Equal(3, result.Value.Stores.Count);
        Assert.Equal("Store 1", result.Value.Stores[0].Name);
        Assert.Equal("Store 2", result.Value.Stores[1].Name);
        Assert.Equal("Store 3", result.Value.Stores[2].Name);
        _mockChainRepository.Verify(r => r.AddAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidChainAndStores_AssignsChainIdToAllStores()
    {
        // Arrange
        var stores = new List<CreateStoreCommand>
        {
            new CreateStoreCommand(
                null,
                1,
                "Store 1",
                Address.Create("123 Main St", "12345", "TestCity").Value,
                PhoneNumber.Create("+1", "1234567890").Value,
                Email.Create("store1@example.com").Value,
                FullName.Create("John", "Doe").Value),
            new CreateStoreCommand(
                null,
                2,
                "Store 2",
                Address.Create("456 Oak Ave", "67890", "AnotherCity").Value,
                PhoneNumber.Create("+1", "0987654321").Value,
                Email.Create("store2@example.com").Value,
                FullName.Create("Jane", "Smith").Value)
        };
        var command = new CreateChainCommand("Test Chain", stores);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value.Stores);
        var chainId = result.Value.Id;
        Assert.All(result.Value.Stores, store => Assert.Equal(chainId, store.ChainId));
    }

    [Fact]
    public async Task Handle_WithValidChainAndStores_PopulatesAllStoreProperties()
    {
        // Arrange
        var stores = new List<CreateStoreCommand>
        {
            new CreateStoreCommand(
                null,
                42,
                "Test Store",
                Address.Create("789 Pine Rd", "54321", "CityName").Value,
                PhoneNumber.Create("+44", "2012345678").Value,
                Email.Create("teststore@example.com").Value,
                FullName.Create("Alice", "Williams").Value)
        };
        var command = new CreateChainCommand("Test Chain", stores);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        var storeDto = result.Value.Stores[0];
        Assert.Equal(42, storeDto.Number);
        Assert.Equal("Test Store", storeDto.Name);
        Assert.Equal("789 Pine Rd", storeDto.Street);
        Assert.Equal("54321", storeDto.PostalCode);
        Assert.Equal("CityName", storeDto.City);
        Assert.Equal("+44", storeDto.CountryCode);
        Assert.Equal("2012345678", storeDto.PhoneNumber);
        Assert.Equal("teststore@example.com", storeDto.Email);
        Assert.Equal("Alice", storeDto.FirstName);
        Assert.Equal("Williams", storeDto.LastName);
    }

    [Fact]
    public async Task Handle_WithValidChainAndStores_SetsCreatedOnAndModifiedOnForStores()
    {
        // Arrange
        var stores = new List<CreateStoreCommand>
        {
            new CreateStoreCommand(
                null,
                1,
                "Store 1",
                Address.Create("123 Main St", "12345", "TestCity").Value,
                PhoneNumber.Create("+1", "1234567890").Value,
                Email.Create("store1@example.com").Value,
                FullName.Create("John", "Doe").Value)
        };
        var command = new CreateChainCommand("Test Chain", stores);
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await _handler.Handle(command);

        // Assert
        var afterCreation = DateTime.UtcNow;
        Assert.True(result.Success);
        var storeDto = result.Value.Stores[0];
        Assert.True(storeDto.CreatedOn >= beforeCreation && storeDto.CreatedOn <= afterCreation);
        Assert.True(storeDto.ModifiedOn >= beforeCreation && storeDto.ModifiedOn <= afterCreation);
    }

    #endregion

    #region Failure Tests

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Handle_WithInvalidChainName_ReturnsFailureResult(string invalidName)
    {
        // Arrange
        var command = new CreateChainCommand(invalidName, null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command));
        _mockChainRepository.Verify(r => r.AddAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithNullChainName_ThrowsException()
    {
        // Arrange
        var command = new CreateChainCommand(null, null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _handler.Handle(command));
        _mockChainRepository.Verify(r => r.AddAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region CancellationToken Tests

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var command = new CreateChainCommand("Test Chain", null);
        var cancellationToken = new CancellationToken();

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mockChainRepository.Verify(
            r => r.AddAsync(It.IsAny<ChainEntity>(), cancellationToken),
            Times.Once);
    }

    #endregion
}
