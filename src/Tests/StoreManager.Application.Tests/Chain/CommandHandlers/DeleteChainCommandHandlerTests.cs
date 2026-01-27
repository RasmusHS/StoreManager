using Helpers;
using Moq;
using StoreManager.Application.Commands.Chain;
using StoreManager.Application.Commands.Chain.Handlers;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Chain.CommandHandlers;

[TestClass]
public class DeleteChainCommandHandlerTests
{
    private readonly Mock<IChainRepository> _mockChainRepository;
    private readonly DeleteChainCommandHandler _handler;

    public DeleteChainCommandHandlerTests()
    {
        _mockChainRepository = new Mock<IChainRepository>();
        _handler = new DeleteChainCommandHandler(_mockChainRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidChainIdAndNoStores_ReturnsSuccessResult()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteChainCommand(chainId);
        var chainEntity = ChainEntity.Create("Test Chain").Value;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainId))
            .ReturnsAsync(0);

        _mockChainRepository
            .Setup(r => r.DeleteAsync(chainId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.DeleteAsync(chainId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentChainId_ReturnsFailureResultWithNotFoundError()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteChainCommand(chainId);

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync((ChainEntity)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal($"Could not find entity with ID {chainId}.", result.Error.Message); 
        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(It.IsAny<ChainId>()), Times.Never);
        _mockChainRepository.Verify(r => r.DeleteAsync(It.IsAny<ChainId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithChainThatHasStores_ReturnsFailureResultWithChainHasStoresError()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteChainCommand(chainId);
        var chainEntity = ChainEntity.Create("Test Chain").Value;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainId))
            .ReturnsAsync(3);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal("The chain could not be deleted due to still having stores. Delete all stores first.", result.Error.Message);
        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.DeleteAsync(It.IsAny<ChainId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithChainThatHasOneStore_ReturnsFailureResultWithChainHasStoresError()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteChainCommand(chainId);
        var chainEntity = ChainEntity.Create("Test Chain").Value;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainId))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal("The chain could not be deleted due to still having stores. Delete all stores first.", result.Error.Message);
        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.DeleteAsync(It.IsAny<ChainId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesCancellationTokenToDeleteAsync()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteChainCommand(chainId);
        var chainEntity = ChainEntity.Create("Test Chain").Value;
        var cancellationToken = new CancellationToken();

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainId))
            .ReturnsAsync(0);

        _mockChainRepository
            .Setup(r => r.DeleteAsync(chainId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.Success);
        _mockChainRepository.Verify(r => r.DeleteAsync(chainId, cancellationToken), Times.Once);
    }
}
