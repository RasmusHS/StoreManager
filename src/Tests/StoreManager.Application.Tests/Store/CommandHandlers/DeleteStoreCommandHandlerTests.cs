using Helpers;
using Moq;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Commands.Store.Handlers;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Store.CommandHandlers;

[TestClass]
public class DeleteStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly DeleteStoreCommandHandler _handler;

    public DeleteStoreCommandHandlerTests()
    {
        _mockStoreRepository = new Mock<IStoreRepository>();
        _handler = new DeleteStoreCommandHandler(_mockStoreRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidStoreId_ReturnsSuccessResult()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var command = new DeleteStoreCommand(storeId);
        var storeEntity = StoreEntity.Create(
            null,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "Test City"),
            PhoneNumber.Create("+1", "1234567890"),
            Email.Create("test@example.com"),
            FullName.Create("John", "Doe")
        ).Value;

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(storeEntity);

        _mockStoreRepository
            .Setup(r => r.DeleteAsync(storeId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeId), Times.Once);
        _mockStoreRepository.Verify(r => r.DeleteAsync(storeId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentStoreId_ReturnsFailureResultWithNotFoundError()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var command = new DeleteStoreCommand(storeId);

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync((StoreEntity)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal($"Could not find entity with ID {storeId}.", result.Error.Message);
        _mockStoreRepository.Verify(r => r.GetByIdAsync(storeId), Times.Once);
        _mockStoreRepository.Verify(r => r.DeleteAsync(It.IsAny<StoreId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesCancellationTokenToRepository()
    {
        // Arrange
        var storeId = StoreId.Create().Value;
        var command = new DeleteStoreCommand(storeId);
        var cancellationToken = new CancellationToken();
        var storeEntity = StoreEntity.Create(
            null,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "Test City"),
            PhoneNumber.Create("+1", "1234567890"),
            Email.Create("test@example.com"),
            FullName.Create("John", "Doe")
        ).Value;

        _mockStoreRepository
            .Setup(r => r.GetByIdAsync(storeId))
            .ReturnsAsync(storeEntity);

        _mockStoreRepository
            .Setup(r => r.DeleteAsync(storeId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.Success);
        _mockStoreRepository.Verify(r => r.DeleteAsync(storeId, cancellationToken), Times.Once);
    }
}
