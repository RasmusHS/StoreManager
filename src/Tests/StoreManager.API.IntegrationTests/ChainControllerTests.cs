using Helpers;
using StoreManager.API.Utilities;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.DTO.Chain.Query;
using StoreManager.Application.DTO.Store.Command;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using ChainResponseDto = StoreManager.Application.DTO.Chain.Command.ChainResponseDto;
//using Envelope = Helpers.Envelope;
//using Envelope = Helpers.Envelope<T>;

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
        var chainName = "Test Chain";
        var request = new CreateChainDto(chainName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chains/postChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var chainResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(chainResponse);
        Assert.NotNull(chainResponse.Result);
        Assert.NotEqual(Guid.Empty, chainResponse.Result.Id);
        Assert.Equal(chainName, chainResponse.Result.Name);
        Assert.Null(chainResponse.Result.Stores);
    }

    [Fact]
    public async Task CreateChain_WithValidDataAndStores_ReturnsOkResult()
    {
        // Arrange
        var chainName = "Test Chain With Stores";
        var stores = new List<CreateStoreDto>
        {
            new CreateStoreDto(null, 101, "Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "store1@test.com", "John", "Doe")
        };
        var request = new CreateChainDto(chainName, stores);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chains/postChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var chainResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(chainResponse);
        Assert.NotNull(chainResponse.Result);
        Assert.NotEqual(Guid.Empty, chainResponse.Result.Id);
        Assert.Equal(chainName, chainResponse.Result.Name);
        Assert.NotNull(chainResponse.Result.Stores);
        Assert.Single(chainResponse.Result.Stores);
        Assert.Equal("Store 1", chainResponse.Result.Stores.First().Name);
        Assert.Equal(101, chainResponse.Result.Stores.First().Number);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateChain_WithEmptyName_ReturnsBadRequest(string name)
    {
        // Arrange
        var request = new CreateChainDto(name);

        // Act
        var response = await _client.PostAsJsonAsync("/api/chains/postChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task CreateChain_WithNameExceeding100Characters_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateChainDto(StringRandom.GetRandomString(101));

        // Act
        var response = await _client.PostAsJsonAsync("/api/chains/postChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
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
        var response = await _client.PostAsJsonAsync("/api/chains/postChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotEmpty(responseBody);
        Assert.Contains("error", responseBody, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region GetChainById Tests

    [Fact]
    public async Task GetChainById_WithExistingChainId_ReturnsOkWithChain()
    {
        // Arrange
        var chainName = StringRandom.GetRandomString(10);
        var chainId = await ApiHelper.CreateChainAndGetId(_client, chainName);
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Act
        var response = await _client.GetAsync($"/api/chains/getChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);

        var chainResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryChainDto>>();
        Assert.NotNull(chainResponse);
        Assert.NotNull(chainResponse.Result);
        Assert.Equal(chainId, chainResponse.Result.Id);
        Assert.Equal(chainName, chainResponse.Result.Name);
    }

    [Fact]
    public async Task GetChainById_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/chains/getChain/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryChainDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    #endregion

    #region GetChainIncludeStores Tests

    [Fact]
    public async Task GetChainIncludeStores_WithExistingChainAndStores_ReturnsOkWithChainAndStores()
    {
        // Arrange
        var chainName = StringRandom.GetRandomString(10);
        var chainId = await ApiHelper.CreateChainAndGetId(_client, chainName);
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Create two stores for the chain
        var createStore1Dto = new CreateStoreDto(chainId, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/stores/postStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        var createStore2Dto = new CreateStoreDto(chainId, 2, "Test Store 2", "456 Main St", "67890", "Test City", "1", "5559876543", "test2@store.com", "Jane", "Doe");
        var store2Response = await _client.PostAsJsonAsync("/api/stores/postStore", createStore2Dto);
        store2Response.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/api/chains/getChainAndStores/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);

        var chainResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryChainDto>>();
        Assert.NotNull(chainResponse);
        Assert.NotNull(chainResponse.Result);
        Assert.Equal(chainId, chainResponse.Result.Id);
        Assert.Equal(chainName, chainResponse.Result.Name);
        Assert.NotNull(chainResponse.Result.Stores);
        Assert.Equal(2, chainResponse.Result.Stores.Count);
        Assert.Contains(chainResponse.Result.Stores, s => s.Name == "Test Store 1");
        Assert.Contains(chainResponse.Result.Stores, s => s.Name == "Test Store 2");
    }

    [Fact]
    public async Task GetChainIncludeStores_WithExistingChainNoStores_ReturnsOkWithChainAndNoStores()
    {
        // Arrange
        var chainName = StringRandom.GetRandomString(10);
        var chainId = await ApiHelper.CreateChainAndGetId(_client, chainName);
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Act
        var response = await _client.GetAsync($"/api/chains/getChainAndStores/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert 
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);

        var chainResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryChainDto>>();
        Assert.NotNull(chainResponse);
        Assert.NotNull(chainResponse.Result);
        Assert.Equal(chainId, chainResponse.Result.Id);
        Assert.Equal(chainName, chainResponse.Result.Name);
        Assert.Empty(chainResponse.Result.Stores);
    }

    [Fact]
    public async Task GetChainIncludeStores_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/chains/getChainAndStores/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryChainDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    #endregion

    #region UpdateChain Tests

    [Fact]
    public async Task UpdateChain_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");

        var updatedName = "Updated Chain Name";
        var request = new UpdateChainDto(chain.Result.Id, updatedName, chain.Result.CreatedOn, chain.Result.ModifiedOn);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chains/putChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);

        var chainResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(chainResponse);
        Assert.NotNull(chainResponse.Result);
        Assert.Equal(chain.Result.Id, chainResponse.Result.Id);
        Assert.Equal(updatedName, chainResponse.Result.Name);
    }

    [Fact]
    public async Task UpdateChain_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateChainDto(Guid.Empty, "Updated Chain Name", DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chains/putChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
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
        var response = await _client.PutAsJsonAsync("/api/chains/putChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task UpdateChain_WithNameExceeding100Characters_ReturnsBadRequest()
    {
        // Arrange
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");

        var request = new UpdateChainDto(chain.Result.Id, StringRandom.GetRandomString(101), chain.Result.CreatedOn, chain.Result.ModifiedOn);

        // Act
        var response = await _client.PutAsJsonAsync("/api/chains/putChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task UpdateChain_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateChainDto(Guid.NewGuid(), "Updated Chain", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/chains/putChain", request);
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    #endregion

    #region DeleteChain Tests

    [Fact]
    public async Task DeleteChain_WithValidChainNoStores_ReturnsOkResult()
    {
        // Arrange
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");

        // Act
        var response = await _client.DeleteAsync($"/api/chains/deleteChain/{chain.Result.Id}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        var chainGetResponse = await _client.GetAsync($"/api/chains/getChain/{chain.Result.Id}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // The chain should no longer exist
        var chainResponse = await chainGetResponse.Content.ReadFromJsonAsync<Helpers.Envelope<QueryChainDto>>();
        Assert.NotNull(chainResponse);
        Assert.Null(chainResponse.Result);
        Assert.Equal(HttpStatusCode.BadRequest, chainGetResponse.StatusCode);

    }

    [Fact]
    public async Task DeleteChain_WithEmptyId_ReturnsBadRequest()
    {
        // Arrange
        var invalidChainId = Guid.Empty;

        // Act
        var response = await _client.DeleteAsync($"/api/chains/deleteChain/{invalidChainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteChain_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingChainId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/chains/deleteChain/{nonExistingChainId}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteChain_WithAssociatedStores_ReturnsBadRequest()
    {
        // Arrange - A chain can only be deleted if it has no associated stores
        var chain = await ApiHelper.CreateChain(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chain.Result.Id}");
        
        var createStore1Dto = new CreateStoreDto(chain.Result.Id, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1 = await ApiHelper.CreateStore(_client, createStore1Dto);

        var createStore2Dto = new CreateStoreDto(chain.Result.Id, 2, "Test Store 2", "456 Main St", "67890", "Test City", "1", "5559876543", "test2@store.com", "Jane", "Doe");
        var store2 = await ApiHelper.CreateStore(_client, createStore2Dto);

        // Act
        var response = await _client.DeleteAsync($"/api/chains/deleteChain/{chain.Result.Id}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<ChainResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    #endregion
}

