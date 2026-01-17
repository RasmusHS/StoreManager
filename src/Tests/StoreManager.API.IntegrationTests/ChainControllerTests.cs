using Helpers;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.DTO.Store.Command;
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
            new CreateStoreDto(null, 101, "Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "store1@test.com", "John", "Doe")
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
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Act
        var response = await _client.GetAsync($"/api/chain/getChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task GetChainById_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChain/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region GetChainIncludeStores Tests

    [Fact]
    public async Task GetChainIncludeStores_WithExistingChainAndStores_ReturnsOkWithChainAndStores()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Create two stores for the chain
        var createStore1Dto = new CreateStoreDto(chainId, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/store/createStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        var createStore2Dto = new CreateStoreDto(chainId, 2, "Test Store 2", "456 Main St", "67890", "Test City", "1", "5559876543", "test2@store.com", "Jane", "Doe");
        var store2Response = await _client.PostAsJsonAsync("/api/store/createStore", createStore2Dto);
        store2Response.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChainAndStores/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task GetChainIncludeStores_WithExistingChainNoStores_ReturnsBadRequest()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Act
        var response = await _client.GetAsync($"/api/chain/getChainAndStores/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert 
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task GetChainIncludeStores_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/chain/getChainAndStores/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region UpdateChain Tests

    [Fact]
    public async Task UpdateChain_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");

        var request = new UpdateChainDto(chain.Result.Id, "Updated Chain Name", chain.Result.CreatedOn, chain.Result.ModifiedOn);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chain/updateChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);
        Assert.Contains("Updated Chain Name", responseBody);
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
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");

        var request = new UpdateChainDto(chain.Result.Id, name, chain.Result.CreatedOn, chain.Result.ModifiedOn);

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
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");

        var request = new UpdateChainDto(chain.Result.Id, StringRandom.GetRandomString(101), chain.Result.CreatedOn, chain.Result.ModifiedOn);

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
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region DeleteChain Tests

    [Fact]
    public async Task DeleteChain_WithValidChainNoStores_ReturnsOkResult()
    {
        // Arrange
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");

        var request = new DeleteChainDto(chain.Result.Id, chain.Result.CreatedOn, chain.Result.ModifiedOn);

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
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");
        var createStore1Dto = new CreateStoreDto(chain.Result.Id, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/store/createStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        var createStore2Dto = new CreateStoreDto(chain.Result.Id, 2, "Test Store 2", "456 Main St", "67890", "Test City", "1", "5559876543", "test2@store.com", "Jane", "Doe");
        var store2Response = await _client.PostAsJsonAsync("/api/store/createStore", createStore2Dto);
        store2Response.EnsureSuccessStatusCode();

        var request = new DeleteChainDto(chain.Result.Id, chain.Result.CreatedOn, chain.Result.ModifiedOn);

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

