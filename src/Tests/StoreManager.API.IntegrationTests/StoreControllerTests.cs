using Helpers;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Commands.Store;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit.Abstractions;

namespace StoreManager.API.IntegrationTests;

public class StoreControllerTests : BaseIntegrationTest
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public StoreControllerTests(StoreManagerWebApplicationFactory factory, ITestOutputHelper output) : base(factory)
    {
        _client = factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task CreateStore_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var request = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/store/createStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData(103, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "invalid-mail.com", "John", "Doe")]
    [InlineData(103, "Test Store", "123 Main St", "12345", "Test City", "1", "abc", "test@store.com", "John", "Doe")]
    public async Task CreateStore_WithInvalidInput_ReturnsBadRequest(int number, string name, string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName)
    {
        // Arrange
        var request = new CreateStoreDto(null, number, name, street, postalCode, city, countryCode, phoneNumber, email, firstName, lastName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/store/createStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStore_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateStoreDto
        {
            ChainId = null,
            Number = 105,
            Name = null
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/store/createStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStore_WithExistingStoreId_ReturnsOkWithStore()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        // Act
        var response = await _client.GetAsync($"/api/store/getStore/{storeId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);
    }

    [Fact]
    public async Task GetStore_WithNonExistingStoreId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/store/getStore/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithExistingChain_ReturnsOkWithStores()
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
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithExistingChainId_ReturnsOkWithOneStore()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Create 1 stores for the chain
        var createStore1Dto = new CreateStoreDto(chainId, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/store/createStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithNonExistingChainId_ReturnsKeyNotFoundException()
    {
        // Arrange
        var chain = ChainId.GetExisting(Guid.NewGuid()).Value;

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chain.Value}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithChainHavingNoStores_ReturnsBadRequest()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithNonExistingChainId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var request = new UpdateStoreDto(storeId, null, 201, "Updated Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "updated@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var request = new UpdateStoreDto(Guid.NewGuid(), null, 202, "Non-existing Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "updated@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var request = new UpdateStoreDto(storeId, null, 203, "Updated Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "invalid-email", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithInvalidValidationData_ReturnsBadRequestWithErrors()
    {
        // Arrange
        var request = new UpdateStoreDto
        {
            Id = Guid.Empty,
            ChainId = Guid.Empty,
            Number = 0,
            Name = null
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStore_WithExistingStoreId_ReturnsOkResult()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var request = new DeleteStoreDto(storeId, DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri(_client.BaseAddress + "api/store/deleteStore"),
            Content = JsonContent.Create(request)
        };
        var response = await _client.SendAsync(httpRequestMessage);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeleteStoreDto(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(request)
        });
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStore_WithInvalidDto_ReturnsBadRequestWithValidationErrors()
    {
        // Arrange
        var request = new DeleteStoreDto(Guid.Empty, DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(request)
        });
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllStores_WithExistingChainId_DeletesAllStoresForChain()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");
        var createStore1Dto = new CreateStoreDto(chainId, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/store/createStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        var createStore2Dto = new CreateStoreDto(chainId, 2, "Test Store 2", "456 Main St", "67890", "Test City", "1", "5559876543", "test2@store.com", "Jane", "Doe");
        var store2Response = await _client.PostAsJsonAsync("/api/store/createStore", createStore2Dto);
        store2Response.EnsureSuccessStatusCode();

        var request = new DeleteAllStoresDto(chainId, DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteAllStores")
        {
            Content = JsonContent.Create(request)
        });
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllStores_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeleteAllStoresDto(Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteAllStores")
        {
            Content = JsonContent.Create(request)
        });
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllStores_WithChainHavingNoStores_ReturnsOk()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        var request = new DeleteAllStoresDto(chainId, DateTime.UtcNow, DateTime.UtcNow);

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteAllStores")
        {
            Content = JsonContent.Create(request)
        });
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateStore_ConcurrentRequests_BothSucceed()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}"); ;
        
        var request1 = new CreateStoreDto(chainId, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var request2 = new CreateStoreDto(chainId, 102, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");

        // Act
        var task1 = _client.PostAsJsonAsync("/api/store/createStore", request1);
        var task2 = _client.PostAsJsonAsync("/api/store/createStore", request2);
        var responses = await Task.WhenAll(task1, task2);
        foreach (var response in responses)// log full server error for debugging
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseBody);
        }

        // Assert
        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }

    [Fact]
    public async Task UpdateStore_AfterDelete_ReturnsBadRequest()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        // Delete the store
        var deleteRequest = new DeleteStoreDto(storeId, DateTime.UtcNow, DateTime.UtcNow);
        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(deleteRequest)
        });

        // Attempt to update
        var updateRequest = new UpdateStoreDto(storeId, null, 401, "Updated Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "updated@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", updateRequest);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStore_AfterSuccessfulUpdate_ReturnsUpdatedData()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var updatedName = "Newly Updated Store";
        var updateRequest = new UpdateStoreDto(storeId, null, 203, updatedName, "456 Updated St", "54321", "Updated City", "1", "5559876543", "new@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        await _client.PutAsJsonAsync("/api/store/updateStore", updateRequest);
        var response = await _client.GetAsync($"/api/store/getStore/{storeId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(updatedName, content);
    }
}
