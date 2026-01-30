using Helpers;
using Moq;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.Queries.Chain;
using StoreManager.Application.Queries.Chain.Handlers;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Chain.QueryHandlers;

[TestClass]
public class GetChainQueryHandlerTests
{
    private readonly Mock<IChainRepository> _mockChainRepository;
    private readonly GetChainQueryHandler _handler;

    public GetChainQueryHandlerTests()
    {
        _mockChainRepository = new Mock<IChainRepository>();
        _handler = new GetChainQueryHandler(_mockChainRepository.Object);
    }

    [Fact]
    public async Task Handle_WithValidChainId_ReturnsSuccessResult()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Test Chain");
        var storeCount = 5;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Value.Id))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id))
            .ReturnsAsync(storeCount);

        var query = new GetChainQuery(chainEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(chainEntity.Value.Id.Value, result.Value.Id);
        Assert.Equal(chainEntity.Value.Name, result.Value.Name);
        Assert.Equal(storeCount, result.Value.StoreCount);
        Assert.Equal(chainEntity.Value.CreatedOn, result.Value.CreatedOn);
        Assert.Equal(chainEntity.Value.ModifiedOn, result.Value.ModifiedOn);
        Assert.Null(result.Value.Stores);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Value.Id), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentChainId_ReturnsFailureResult()
    {
        // Arrange
        var chainId = ChainId.GetExisting(Guid.NewGuid()).Value;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync((ChainEntity)null);

        var query = new GetChainQuery(chainId);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Equal($"Could not find entity with ID {chainId}.", result.Error.Message);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(It.IsAny<ChainId>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithChainHavingZeroStores_ReturnsSuccessResultWithZeroStoreCount()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Empty Chain");
        var storeCount = 0;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Value.Id))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id))
            .ReturnsAsync(storeCount);

        var query = new GetChainQuery(chainEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(0, result.Value.StoreCount);
        Assert.Equal(chainEntity.Value.Id.Value, result.Value.Id);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Value.Id), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_WithChainHavingMultipleStores_ReturnsCorrectStoreCount()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Large Chain");
        var storeCount = 100;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Value.Id))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id))
            .ReturnsAsync(storeCount);

        var query = new GetChainQuery(chainEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(100, result.Value.StoreCount);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Value.Id), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidRequest_StoresPropertyIsNull()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Test Chain");
        var storeCount = 3;

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Value.Id))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id))
            .ReturnsAsync(storeCount);

        var query = new GetChainQuery(chainEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Value.Stores);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenCorrectly()
    {
        // Arrange
        var chainEntity = ChainEntity.Create("Test Chain");
        var storeCount = 2;
        var cancellationToken = new CancellationToken();

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainEntity.Value.Id))
            .ReturnsAsync(chainEntity);

        _mockChainRepository
            .Setup(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id))
            .ReturnsAsync(storeCount);

        var query = new GetChainQuery(chainEntity.Value.Id);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);

        _mockChainRepository.Verify(r => r.GetByIdAsync(chainEntity.Value.Id), Times.Once);
        _mockChainRepository.Verify(r => r.GetCountofStoresByChainAsync(chainEntity.Value.Id), Times.Once);
    }
}
