using Helpers;
using Moq;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.Commands.Store.Handlers;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace StoreManager.Application.Tests.Store.CommandHandlers;

[TestClass]
public class CreateStoreCommandHandlerTests
{
    private readonly Mock<IStoreRepository> _mockStoreRepository;
    private readonly CreateStoreCommandHandler _handler;

    public CreateStoreCommandHandlerTests()
    {
        _mockStoreRepository = new Mock<IStoreRepository>();
        _handler = new CreateStoreCommandHandler(_mockStoreRepository.Object);
    }

    #region Successful Creation Tests

    [Fact]
    public async Task Handle_WithValidStoreDataWithoutChainId_ReturnsSuccessResult()
    {
        // Arrange
        var command = new CreateStoreCommand(
            null,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Store", result.Value.Name);
        Assert.Equal(1, result.Value.Number);
        Assert.Null(result.Value.ChainId);
        Assert.Equal("123 Main St", result.Value.Street);
        Assert.Equal("12345", result.Value.PostalCode);
        Assert.Equal("TestCity", result.Value.City);
        Assert.Equal("+1", result.Value.CountryCode);
        Assert.Equal("1234567890", result.Value.PhoneNumber);
        Assert.Equal("store@example.com", result.Value.Email);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        _mockStoreRepository.Verify(r => r.AddAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidStoreDataWithChainId_ReturnsSuccessResult()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new CreateStoreCommand(
            chainId,
            2,
            "Chain Store",
            Address.Create("456 Oak Ave", "67890", "AnotherCity").Value,
            PhoneNumber.Create("+44", "2087654321").Value,
            Email.Create("chainstore@example.com").Value,
            FullName.Create("Jane", "Smith").Value);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal("Chain Store", result.Value.Name);
        Assert.Equal(2, result.Value.Number);
        Assert.Equal(chainId.Value, result.Value.ChainId);
        Assert.Equal("456 Oak Ave", result.Value.Street);
        Assert.Equal("67890", result.Value.PostalCode);
        Assert.Equal("AnotherCity", result.Value.City);
        Assert.Equal("+44", result.Value.CountryCode);
        Assert.Equal("2087654321", result.Value.PhoneNumber);
        Assert.Equal("chainstore@example.com", result.Value.Email);
        Assert.Equal("Jane", result.Value.FirstName);
        Assert.Equal("Smith", result.Value.LastName);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        _mockStoreRepository.Verify(r => r.AddAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidStoreData_SetsCreatedOnAndModifiedOn()
    {
        // Arrange
        var command = new CreateStoreCommand(
            null,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = await _handler.Handle(command);

        // Assert
        var afterCreation = DateTime.UtcNow;
        Assert.True(result.Success);
        Assert.True(result.Value.CreatedOn >= beforeCreation && result.Value.CreatedOn <= afterCreation);
        Assert.True(result.Value.ModifiedOn >= beforeCreation && result.Value.ModifiedOn <= afterCreation);
    }

    [Fact]
    public async Task Handle_WithValidStoreData_CallsRepositoryAddAsyncOnce()
    {
        // Arrange
        var command = new CreateStoreCommand(
            null,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);

        // Act
        await _handler.Handle(command);

        // Assert
        _mockStoreRepository.Verify(r => r.AddAsync(
            It.Is<StoreEntity>(s =>
                s.Number == 1 &&
                s.Name == "Test Store" &&
                s.Email.Value == "store@example.com"),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithCancellationToken_PassesCancellationTokenToRepository()
    {
        // Arrange
        var command = new CreateStoreCommand(
            null,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);
        var cancellationToken = new CancellationToken();

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mockStoreRepository.Verify(r => r.AddAsync(
            It.IsAny<StoreEntity>(),
            cancellationToken), Times.Once);
    }

    #endregion

    #region Failure Tests

    [Fact]
    public async Task Handle_WithEmptyStoreName_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateStoreCommand(
            null,
            1,
            "",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Failure);
        Assert.NotNull(result.Error);
        _mockStoreRepository.Verify(r => r.AddAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WithWhiteSpaceStoreName_ReturnsFailureResult()
    {
        // Arrange
        var command = new CreateStoreCommand(
            null,
            1,
            "   ",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Failure);
        Assert.NotNull(result.Error);
        _mockStoreRepository.Verify(r => r.AddAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenStoreEntityCreationFails_ReturnsFailureResult()
    {
        // Arrange - Using empty address street to trigger validation failure

        // Since the value objects might validate themselves, we test with the assumption
        // that StoreEntity.Create could fail
        var command = new CreateStoreCommand(
            null,
            1,
            "Test Store",
            Address.Create("", "12345", "TestCity").Value, // This might be caught by Address validation
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Failure);
        _mockStoreRepository.Verify(r => r.AddAsync(It.IsAny<StoreEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region DTO Mapping Tests

    [Fact]
    public async Task Handle_MapsAllPropertiesCorrectly()
    {
        // Arrange
        var chainId = ChainId.Create().Value;
        var command = new CreateStoreCommand(
            chainId,
            42,
            "Complete Store",
            Address.Create("789 Elm St", "11111", "ThirdCity").Value,
            PhoneNumber.Create("+33", "0123456789").Value,
            Email.Create("complete@example.com").Value,
            FullName.Create("Robert", "Johnson").Value);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        var dto = result.Value;
        Assert.NotEqual(Guid.Empty, dto.Id);
        Assert.Equal(chainId.Value, dto.ChainId);
        Assert.Equal(42, dto.Number);
        Assert.Equal("Complete Store", dto.Name);
        Assert.Equal("789 Elm St", dto.Street);
        Assert.Equal("11111", dto.PostalCode);
        Assert.Equal("ThirdCity", dto.City);
        Assert.Equal("+33", dto.CountryCode);
        Assert.Equal("0123456789", dto.PhoneNumber);
        Assert.Equal("complete@example.com", dto.Email);
        Assert.Equal("Robert", dto.FirstName);
        Assert.Equal("Johnson", dto.LastName);
        Assert.NotNull(dto.CreatedOn);
        Assert.NotNull(dto.ModifiedOn);
    }

    [Fact]
    public async Task Handle_WithNullChainId_MapsChainIdAsNull()
    {
        // Arrange
        var command = new CreateStoreCommand(
            null,
            1,
            "Test Store",
            Address.Create("123 Main St", "12345", "TestCity").Value,
            PhoneNumber.Create("+1", "1234567890").Value,
            Email.Create("store@example.com").Value,
            FullName.Create("John", "Doe").Value);

        // Act
        var result = await _handler.Handle(command);

        // Assert
        Assert.True(result.Success);
        Assert.Null(result.Value.ChainId);
    }

    #endregion
}
