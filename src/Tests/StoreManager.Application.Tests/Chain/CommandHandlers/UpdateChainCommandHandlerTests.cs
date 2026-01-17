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
public class UpdateChainCommandHandlerTests
{
    private readonly Mock<IChainRepository> _mockChainRepository;
    private readonly UpdateChainCommandHandler _handler;

    public UpdateChainCommandHandlerTests()
    {
        _mockChainRepository = new Mock<IChainRepository>();
        _handler = new UpdateChainCommandHandler(_mockChainRepository.Object);
    }

    #region Update Chain Success Tests

    [Fact]
    public async Task Handle_WithValidIdAndName_ReturnsSuccessResult()
    {
        // Arrange
        var existingChain = ChainEntity.Create("Original Chain Name").Value;
        var command = new UpdateChainCommand(existingChain.Id, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(existingChain.Id))
            .ReturnsAsync(existingChain);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Updated Chain Name", result.Value.Name);
        Assert.Equal(existingChain.Id.Value, result.Value.Id);
        _mockChainRepository.Verify(r => r.GetByIdAsync(existingChain.Id), Times.Once);
        _mockChainRepository.Verify(r => r.UpdateAsync(existingChain, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidUpdate_UpdatesModifiedOn()
    {
        // Arrange
        var existingChain = ChainEntity.Create("Original Chain Name").Value;
        var originalModifiedOn = existingChain.ModifiedOn;
        var command = new UpdateChainCommand(existingChain.Id, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(existingChain.Id))
            .ReturnsAsync(existingChain);

        // Wait a small amount to ensure time difference
        await Task.Delay(10);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.ModifiedOn > originalModifiedOn);
        Assert.Equal(existingChain.CreatedOn, result.Value.CreatedOn);
    }

    [Fact]
    public async Task Handle_WithValidUpdate_ReturnsCorrectResponseDto()
    {
        // Arrange
        var existingChain = ChainEntity.Create("Original Chain Name").Value;
        var command = new UpdateChainCommand(existingChain.Id, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(existingChain.Id))
            .ReturnsAsync(existingChain);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(existingChain.Id.Value, result.Value.Id);
        Assert.Equal("Updated Chain Name", result.Value.Name);
        Assert.Equal(existingChain.CreatedOn, result.Value.CreatedOn);
        Assert.Equal(existingChain.ModifiedOn, result.Value.ModifiedOn);
    }

    [Fact]
    public async Task Handle_WithValidUpdate_CallsRepositoryMethodsInCorrectOrder()
    {
        // Arrange
        var existingChain = ChainEntity.Create("Original Chain Name").Value;
        var command = new UpdateChainCommand(existingChain.Id, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);
        var callOrder = new List<string>();

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(existingChain.Id))
            .ReturnsAsync(existingChain)
            .Callback(() => callOrder.Add("GetByIdAsync"));

        _mockChainRepository
            .Setup(r => r.UpdateAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()))
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

    #region Update Chain Not Found Tests

    [Fact]
    public async Task Handle_WhenChainNotFound_ReturnsFailureResult()
    {
        // Arrange
        var chainId = ChainId.GetExisting(Guid.NewGuid()).Value;
        var command = new UpdateChainCommand(chainId, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync((ChainEntity?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Failure);
        Assert.NotNull(result.Error);
        _mockChainRepository.Verify(r => r.GetByIdAsync(chainId), Times.Once);
        _mockChainRepository.Verify(r => r.UpdateAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenChainNotFound_DoesNotCallUpdateAsync()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new UpdateChainCommand(chainId, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync((ChainEntity?)null);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.False(result.Success);
        _mockChainRepository.Verify(r => r.UpdateAsync(It.IsAny<ChainEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region CancellationToken Tests

    [Fact]
    public async Task Handle_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var existingChain = ChainEntity.Create("Original Chain Name").Value;
        var command = new UpdateChainCommand(chainId, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);
        var cancellationToken = new CancellationToken();

        _mockChainRepository
            .Setup(r => r.GetByIdAsync(chainId))
            .ReturnsAsync(existingChain);

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        Assert.True(result.Success);
        _mockChainRepository.Verify(r => r.UpdateAsync(existingChain, cancellationToken), Times.Once);
    }

    #endregion
}
