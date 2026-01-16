using Helpers;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;

namespace StoreManager.API.IntegrationTests;

public class ChainControllerTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public ChainControllerTests(StoreManagerWebApplicationFactory factory, ITestOutputHelper output) : base(factory)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    #region CreateChain Tests

    [Fact]
    public async Task CreateChain_WithValidDataAndNoStores_ReturnsOkResult()
    {
        // Arrange
        var request = new CreateChainDto("Test Chain");

        // Act
        var response = await _client.PostAsJsonAsync("/api/chain/createChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateChain_WithValidDataAndStores_ReturnsOkResult()
    {
        // Arrange
        var stores = new List<CreateStoreDto>
        {
            new CreateStoreDto(Guid.NewGuid(), 101, "Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "store1@test.com", "John", "Doe")
        };
        var request = new CreateChainDto("Test Chain With Stores", stores);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chain/createChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateChain_WithEmptyName_ReturnsBadRequest(string name)
    {
        // Arrange
        var request = new CreateChainDto(name);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chain/createChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateChain_WithNameExceeding100Characters_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateChainDto(StringRandom.GetRandomString(101));

        // Act
        var response = await _client.PostAsJsonAsync("/api/chain/createChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateChain_WithInvalidStoreData_ReturnsBadRequest()
    {
        // Arrange
        var stores = new List<CreateStoreDto>
        {
            new CreateStoreDto(null, 101, null, "123 Main St", "12345", "Test City", "1", "5551234567", "store1@test.com", "John", "Doe")
        };
        var request = new CreateChainDto("Test Chain With Stores", stores);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chain/createChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region GetChainById Tests

    [Fact]
    public async Task GetChainById_WithExistingChainId_ReturnsOkWithChain()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChain/{chain.Id.Value}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task GetChainById_WithNonExistingChainId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChain/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    #endregion

    #region GetChainIncludeStores Tests

    [Fact]
    public async Task GetChainIncludeStores_WithExistingChainAndStores_ReturnsOkWithChainAndStores()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var stores = new List<StoreEntity>
        {
            StoreEntity.Create(chain.Id, 1, "Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("+1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager1")).Value,
            StoreEntity.Create(chain.Id, 2, "Store 2", Address.Create("456 Test St", "67890", "Test City"), PhoneNumber.Create("+1", "5559876543"), Email.Create("store2@test.com"), FullName.Create("Test", "Manager2")).Value
        };
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddRangeAsync(stores);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChainAndStores/{chain.Id.Value}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task GetChainIncludeStores_WithExistingChainNoStores_ReturnsInternalServerError()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain Without Stores").Value;
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChainAndStores/{chain.Id.Value}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert 
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task GetChainIncludeStores_WithNonExistingChainId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChainAndStores/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    #endregion

    #region UpdateChain Tests

    [Fact]
    public async Task UpdateChain_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var chain = ChainEntity.Create("Original Chain Name").Value;
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var request = new UpdateChainDto(chain.Id.Value, "Updated Chain Name", chain.CreatedOn, DateTime.UtcNow);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chain/updateChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateChain_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateChainDto(Guid.Empty, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chain/updateChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task UpdateChain_WithEmptyName_ReturnsBadRequest(string name)
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var request = new UpdateChainDto(chain.Id.Value, name, chain.CreatedOn, DateTime.UtcNow);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chain/updateChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateChain_WithNameExceeding100Characters_ReturnsBadRequest()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var request = new UpdateChainDto(chain.Id.Value, StringRandom.GetRandomString(101), chain.CreatedOn, DateTime.UtcNow);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chain/updateChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateChain_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateChainDto(Guid.NewGuid(), "Updated Chain", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/chain/updateChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    #endregion

    #region DeleteChain Tests

    [Fact]
    public async Task DeleteChain_WithValidChainNoStores_ReturnsOkResult()
    {
        // Arrange
        var chain = ChainEntity.Create("Chain To Delete").Value;
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var request = new DeleteChainDto(chain.Id.Value, chain.CreatedOn, DateTime.UtcNow);

        // Act
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_client.BaseAddress + "api/chain/deleteChain"),
            Content = JsonContent.Create(request)
        };
        var response = await _client.SendAsync(httpRequestMessage);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert - Note: The actual HTTP method doesn't matter for the assertion, focus on the result
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteChain_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeleteChainDto(Guid.Empty, DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_client.BaseAddress + "api/chain/deleteChain"),
            Content = JsonContent.Create(request)
        };
        var response = await _client.SendAsync(httpRequestMessage);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteChain_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeleteChainDto(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_client.BaseAddress + "api/chain/deleteChain"),
            Content = JsonContent.Create(request)
        };
        var response = await _client.SendAsync(httpRequestMessage);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteChain_WithAssociatedStores_ReturnsBadRequest()
    {
        // Arrange - A chain can only be deleted if it has no associated stores
        var chain = ChainEntity.Create("Chain With Stores").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await using var transaction = await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await transaction.CommitAsync();

        var request = new DeleteChainDto(chain.Id.Value, chain.CreatedOn, DateTime.UtcNow);

        // Act
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_client.BaseAddress + "api/chain/deleteChain"),
            Content = JsonContent.Create(request)
        };
        var response = await _client.SendAsync(httpRequestMessage);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion
}

