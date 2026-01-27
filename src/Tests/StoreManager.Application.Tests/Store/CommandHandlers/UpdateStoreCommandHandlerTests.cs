using Helpers;
using Moq;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Commands.Store.Handlers;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Store.CommandHandlers;

[TestClass]
public class UpdateStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly UpdateStoreCommandHandler _handler;

    public UpdateStoreCommandHandlerTests()
    {
        _mockStoreRepository = new Mock<IStoreRepository>();
        _handler = new UpdateStoreCommandHandler(_mockStoreRepository.Object);
    }

    #region Update Store Success Tests - Without ChainId

    [Fact]
    public async Task Handle_WithValidStoreDataWithoutChainId_ReturnsSuccessResult()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var existingStore = StoreEntity.Create(
            null,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Store", result.Value.Name);
        Assert.Equal(2, result.Value.Number);
        Assert.Null(result.Value.ChainId);
        Assert.Equal("New Street", result.Value.Street);
        Assert.Equal("22222", result.Value.PostalCode);
        Assert.Equal("NewCity", result.Value.City);
        Assert.Equal("+44", result.Value.CountryCode);
        Assert.Equal("2222222222", result.Value.PhoneNumber);
        Assert.Equal("new@example.com", result.Value.Email);
        Assert.Equal("New", result.Value.FirstName);
        Assert.Equal("Owner", result.Value.LastName);
        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeId), Times.Once);
        _mockStoreRepository.Verify(r => r.UpdateAsync(existingStore, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithoutChainId_WhenExistingStoreAlsoHasNoChainId_UpdatesSuccessfully()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var existingStore = StoreEntity.Create(
            null,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Value.ChainId);
        _mockStoreRepository.Verify(r => r.UpdateAsync(existingStore, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Update Store Success Tests - With ChainId

    [Fact]
    public async Task Handle_WithValidStoreDataWithChainId_ReturnsSuccessResult()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var chainId = ChainId.Create().Value;
        var existingStore = StoreEntity.Create(
            chainId,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var newChainId = ChainId.Create().Value;
        var command = new UpdateStoreCommand(
            storeId,
            newChainId,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Store", result.Value.Name);
        Assert.Equal(2, result.Value.Number);
        Assert.Equal(newChainId.Value, result.Value.ChainId);
        Assert.Equal("New Street", result.Value.Street);
        Assert.Equal("22222", result.Value.PostalCode);
        Assert.Equal("NewCity", result.Value.City);
        Assert.Equal("+44", result.Value.CountryCode);
        Assert.Equal("2222222222", result.Value.PhoneNumber);
        Assert.Equal("new@example.com", result.Value.Email);
        Assert.Equal("New", result.Value.FirstName);
        Assert.Equal("Owner", result.Value.LastName);
        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeId), Times.Once);
        _mockStoreRepository.Verify(r => r.UpdateAsync(existingStore, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Update Store Success Tests - ChainId Removal

    [Fact]
    public async Task Handle_RemovingChainId_WhenExistingStoreHasChainId_UpdatesSuccessfully()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var chainId = ChainId.Create().Value;
        var existingStore = StoreEntity.Create(
            chainId,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Value.ChainId);
        _mockStoreRepository.Verify(r => r.UpdateAsync(existingStore, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Update Store Success Tests - Response Mapping

    [Fact]
    public async Task Handle_WithValidUpdate_ReturnsCorrectResponseDto()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var existingStore = StoreEntity.Create(
            null,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var createdOn = DateTime.UtcNow.AddDays(-5);
        var modifiedOn = DateTime.UtcNow;

        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            createdOn,
            modifiedOn);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(existingStore.Id.Value, result.Value.Id);
        Assert.Equal(createdOn, result.Value.CreatedOn);
        Assert.Equal(modifiedOn, result.Value.ModifiedOn);
    }

    [Fact]
    public async Task Handle_WithValidUpdate_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var chainId = ChainId.Create().Value;
        var existingStore = StoreEntity.Create(
            null,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var command = new UpdateStoreCommand(
            storeId,
            chainId,
            99,
            "Test Store Name",
            Address.Create("123 Test Ave", "54321", "TestCity").Value,
            PhoneNumber.Create("+33", "9876543210").Value,
            Email.Create("test@test.com").Value,
            FullName.Create("John", "Doe").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(chainId.Value, result.Value.ChainId);
        Assert.Equal(99, result.Value.Number);
        Assert.Equal("Test Store Name", result.Value.Name);
        Assert.Equal("123 Test Ave", result.Value.Street);
        Assert.Equal("54321", result.Value.PostalCode);
        Assert.Equal("TestCity", result.Value.City);
        Assert.Equal("+33", result.Value.CountryCode);
        Assert.Equal("9876543210", result.Value.PhoneNumber);
        Assert.Equal("test@test.com", result.Value.Email);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
    }

    #endregion

    #region Update Store Not Found Tests

    [Fact]
    public async Task Handle_WhenStoreNotFound_ReturnsFailureResult()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync((StoreEntity?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal($"Could not find entity with ID {storeId}.", result.Error.Message);
        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeId), Times.Once);
        _mockStoreRepository.Verify(r => r.UpdateAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenStoreNotFound_DoesNotCallUpdateAsync()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync((StoreEntity?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal($"Could not find entity with ID {storeId}.", result.Error.Message);
        _mockStoreRepository.Verify(r => r.UpdateAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region CancellationToken Tests

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var existingStore = StoreEntity.Create(
            null,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var cancellationToken = new CancellationToken();

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.Success);
        _mockStoreRepository.Verify(r => r.UpdateAsync(existingStore, cancellationToken), Times.Once);
    }

    #endregion

    #region Repository Method Call Order Tests

    [Fact]
    public async Task Handle_WithValidUpdate_CallsRepositoryMethodsInCorrectOrder()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var existingStore = StoreEntity.Create(
            null,
            1,
            "Original Store",
            Address.Create("Old Street", "11111", "OldCity").Value,
            PhoneNumber.Create("+1", "1111111111").Value,
            Email.Create("old@example.com").Value,
            FullName.Create("Old", "Owner").Value).Value;

        var command = new UpdateStoreCommand(
            storeId,
            null,
            2,
            "Updated Store",
            Address.Create("New Street", "22222", "NewCity").Value,
            PhoneNumber.Create("+44", "2222222222").Value,
            Email.Create("new@example.com").Value,
            FullName.Create("New", "Owner").Value,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var callOrder = new List<string>();

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(existingStore)
            .Callback(() => callOrder.Add("GetByIdAsync"));

        _mockStoreRepository
            .Setup(r => r.UpdateAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Callback(() => callOrder.Add("UpdateAsync"));

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(2, callOrder.Count);
        Assert.Equal("GetByIdAsync", callOrder[0]);
        Assert.Equal("UpdateAsync", callOrder[1]);
    }

    #endregion
}
