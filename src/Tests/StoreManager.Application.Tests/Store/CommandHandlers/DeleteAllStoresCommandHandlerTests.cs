using Helpers;
using Moq;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Commands.Store.Handlers;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Store.CommandHandlers;

[TestClass]
public class DeleteAllStoresCommandHandlerTests
{
    private readonly Mock<IChainRepository> _mockChainRepository;
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly DeleteAllStoresCommandHandler _handler;

    public DeleteAllStoresCommandHandlerTests()
    {
        _mockChainRepository = new Mock<IChainRepository>();
        _mockStoreRepository = new Mock<IStoreRepository>();
        _handler = new DeleteAllStoresCommandHandler(_mockChainRepository.Object, _mockStoreRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidChainId_ReturnsSuccessResult()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteAllStoresCommand(chainId);
        var chainEntity = ChainEntity.Create("Test Chain").Value;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.DeleteByChainIdAsync(chainId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockStoreRepository.Verify(r => r.DeleteByChainIdAsync(chainId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentChainId_ReturnsFailureResultWithNotFoundError()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteAllStoresCommand(chainId);

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
        _mockStoreRepository.Verify(r => r.DeleteByChainIdAsync(It.IsAny<ChainId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithValidChainId_CallsDeleteByChainIdAsyncWithCorrectParameters()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteAllStoresCommand(chainId);
        var chainEntity = ChainEntity.Create("Test Chain").Value;
        var cancellationToken = new CancellationToken();

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.DeleteByChainIdAsync(chainId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.Success);
        _mockStoreRepository.Verify(r => r.DeleteByChainIdAsync(chainId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesCancellationTokenToRepository()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new DeleteAllStoresCommand(chainId);
        var chainEntity = ChainEntity.Create("Test Chain").Value;
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(chainEntity);

        _mockStoreRepository
            .Setup(r => r.DeleteByChainIdAsync(chainId, cancellationToken))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.Success);
        _mockStoreRepository.Verify(r => r.DeleteByChainIdAsync(chainId, cancellationToken), Times.Once);
    }
}
