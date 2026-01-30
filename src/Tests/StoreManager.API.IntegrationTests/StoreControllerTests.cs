using Helpers;
using Microsoft.EntityFrameworkCore;
using StoreManager.API.Utilities;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Chain.ValueObjects;
using System.Net;
using System.Net.Http.Json;
using Xunit.Abstractions;
using StoreResponseDto = StoreManager.Application.DTO.Store.Command.StoreResponseDto;

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
        var request = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "+1", "5551234567", "test@store.com", "John", "Doe");
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/stores/postStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var storeResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(storeResponse);
        Assert.NotNull(storeResponse.Result);
        Assert.NotEqual(Guid.Empty, storeResponse.Result.Id);
        Assert.Equal(request.Number, storeResponse.Result.Number);
        Assert.Equal(request.Name, storeResponse.Result.Name);
        Assert.Equal(request.Street, storeResponse.Result.Street);
        Assert.Equal(request.PostalCode, storeResponse.Result.PostalCode);
        Assert.Equal(request.City, storeResponse.Result.City);
        Assert.Equal(request.CountryCode, storeResponse.Result.CountryCode);
        Assert.Equal(request.PhoneNumber, storeResponse.Result.PhoneNumber);
        Assert.Equal(request.Email, storeResponse.Result.Email);
        Assert.Equal(request.FirstName, storeResponse.Result.FirstName);
        Assert.Equal(request.LastName, storeResponse.Result.LastName);
        Assert.Null(storeResponse.Result.ChainId);
    }

    [Theory]
    [InlineData(103, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "invalid-mail.com", "John", "Doe")]
    [InlineData(103, "Test Store", "123 Main St", "12345", "Test City", "1", "abc", "test@store.com", "John", "Doe")]
    public async Task CreateStore_WithInvalidInput_ReturnsBadRequest(int number, string name, string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName)
    {
        // Arrange
        var request = new CreateStoreDto(null, number, name, street, postalCode, city, countryCode, phoneNumber, email, firstName, lastName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/stores/postStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task CreateStore_WithMissingRequiredFields_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateStoreDto
        {
            ChainId = null,
            Number = 105,
            Name = null!
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/stores/postStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        //_output.WriteLine(errorResponse!.Result!.ToString());
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task GetStore_WithExistingStoreId_ReturnsOkWithStore()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "+1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        // Act
        var response = await _client.GetAsync($"/api/stores/getStore/{storeId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotEmpty(responseBody);

        var queryStoreDto = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryStoreDto>>();
        Assert.NotNull(queryStoreDto);
        Assert.NotNull(queryStoreDto.Result);
        Assert.NotEqual(Guid.Empty, queryStoreDto.Result.Id);
        Assert.Equal(storeDto.Number, queryStoreDto.Result.Number);
        Assert.Equal(storeDto.Name, queryStoreDto.Result.Name);
        Assert.Equal(storeDto.Street, queryStoreDto.Result.Street);
        Assert.Equal(storeDto.PostalCode, queryStoreDto.Result.PostalCode);
        Assert.Equal(storeDto.City, queryStoreDto.Result.City);
        Assert.Equal(storeDto.CountryCode, queryStoreDto.Result.CountryCode);
        Assert.Equal(storeDto.PhoneNumber, queryStoreDto.Result.PhoneNumber);
        Assert.Equal(storeDto.Email, queryStoreDto.Result.Email);
        Assert.Equal(storeDto.FirstName, queryStoreDto.Result.FirstName);
        Assert.Equal(storeDto.LastName, queryStoreDto.Result.LastName);
        Assert.Null(queryStoreDto.Result.ChainId);
    }

    [Fact]
    public async Task GetStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/stores/getStore/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryStoreDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task GetStoresByChainId_WithExistingChain_ReturnsOkWithStores()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Create two stores for the chain
        var createStore1Dto = new CreateStoreDto(chainId, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/stores/postStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        var createStore2Dto = new CreateStoreDto(chainId, 2, "Test Store 2", "456 Main St", "67890", "Test City", "1", "5559876543", "test2@store.com", "Jane", "Doe");
        var store2Response = await _client.PostAsJsonAsync("/api/stores/postStore", createStore2Dto);
        store2Response.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/api/stores/getStoresByChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var queryStoreDto = await response.Content.ReadFromJsonAsync<Helpers.Envelope<CollectionResponseBase<QueryStoreDto>>>();
        Assert.NotNull(queryStoreDto);
        Assert.Equal(2, queryStoreDto.Result.Data.Count());
        Assert.All(queryStoreDto.Result.Data, store =>
        {
            Assert.NotNull(store);
            Assert.Equal(chainId, store.ChainId);
        });
        Assert.Contains(queryStoreDto.Result.Data, s => s.Name == "Test Store 1");
        Assert.Contains(queryStoreDto.Result.Data, s => s.Name == "Test Store 2");
    }

    [Fact]
    public async Task GetStoresByChainId_WithExistingChainId_ReturnsOkWithOneStore()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Create 1 stores for the chain
        var createStore1Dto = new CreateStoreDto(chainId, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/stores/postStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        // Act
        var response = await _client.GetAsync($"/api/stores/getStoresByChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var queryStoreDto = await response.Content.ReadFromJsonAsync<Helpers.Envelope<CollectionResponseBase<QueryStoreDto>>>();
        Assert.NotNull(queryStoreDto);
        Assert.NotNull(queryStoreDto.Result);
        Assert.Single(queryStoreDto.Result.Data);
        var store = queryStoreDto.Result.Data.First();
        Assert.NotNull(store);
        Assert.Equal(chainId, store.ChainId);
        Assert.Equal("Test Store 1", store.Name);
        Assert.Equal(1, store.Number);
    }

    [Fact]
    public async Task GetStoresByChainId_WithChainHavingNoStores_ReturnsBadRequest()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Act
        var response = await _client.GetAsync($"/api/stores/getStoresByChain/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryStoreDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task GetStoresByChainId_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/stores/getStoresByChain/{nonExistingId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryStoreDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task UpdateStore_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "+1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var request = new UpdateStoreDto(storeId, null, 201, "Updated Store", "456 Updated St", "54321", "Updated City", "+1", "5559876543", "updated@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/stores/putStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var storeResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(storeResponse);
        Assert.NotNull(storeResponse.Result);
        Assert.Equal(storeId, storeResponse.Result.Id);
        Assert.Equal(request.Number, storeResponse.Result.Number);
        Assert.Equal(request.Name, storeResponse.Result.Name);
        Assert.Equal(request.Street, storeResponse.Result.Street);
        Assert.Equal(request.PostalCode, storeResponse.Result.PostalCode);
        Assert.Equal(request.City, storeResponse.Result.City);
        Assert.Equal(request.CountryCode, storeResponse.Result.CountryCode);
        Assert.Equal(request.PhoneNumber, storeResponse.Result.PhoneNumber);
        Assert.Equal(request.Email, storeResponse.Result.Email);
        Assert.Equal(request.FirstName, storeResponse.Result.FirstName);
        Assert.Equal(request.LastName, storeResponse.Result.LastName);
        Assert.Null(storeResponse.Result.ChainId);
    }

    [Fact]
    public async Task UpdateStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var request = new UpdateStoreDto(Guid.NewGuid(), null, 202, "Non-existing Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "updated@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/stores/putStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task UpdateStore_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var request = new UpdateStoreDto(storeId, null, 203, "Updated Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "invalid-email", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/stores/putStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
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
        var response = await _client.PutAsJsonAsync("/api/stores/putStore", request);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteStore_WithExistingStoreId_ReturnsOkResult()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        var response = await _client.DeleteAsync($"/api/stores/deleteStore/{storeId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var storeGetResponse = await _client.GetAsync($"/api/stores/getStore/{storeId}");
        var storeResponse = await storeGetResponse.Content.ReadFromJsonAsync<Helpers.Envelope<QueryStoreDto>>();
        Assert.NotNull(storeResponse);
        Assert.Null(storeResponse.Result);
        Assert.NotNull(storeResponse.ErrorMessage);
        Assert.NotEmpty(storeResponse.ErrorMessage);
        _output.WriteLine($"Error: {storeResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingStoreId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/stores/deleteStore/{nonExistingStoreId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteStore_WithInvalidStoreId_ReturnsBadRequestWithValidationErrors()
    {
        // Arrange
        var invalidStoreId = Guid.Empty;

        // Act
        var response = await _client.DeleteAsync($"/api/stores/deleteStore/{invalidStoreId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteAllStores_WithExistingChainId_DeletesAllStoresForChain()
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");
        var createStore1Dto = new CreateStoreDto(chainId, 1, "Test Store 1", "123 Main St", "12345", "Test City", "1", "5551234567", "test1@store.com", "John", "Doe");
        var store1Response = await _client.PostAsJsonAsync("/api/stores/postStore", createStore1Dto);
        store1Response.EnsureSuccessStatusCode();

        var createStore2Dto = new CreateStoreDto(chainId, 2, "Test Store 2", "456 Main St", "67890", "Test City", "1", "5559876543", "test2@store.com", "Jane", "Doe");
        var store2Response = await _client.PostAsJsonAsync("/api/stores/postStore", createStore2Dto);
        store2Response.EnsureSuccessStatusCode();

        // Act
        var response = await _client.DeleteAsync($"/api/stores/deleteAllStores/{chainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var storeGetResponse = await _client.GetAsync($"/api/stores/getStoresByChain/{chainId}");
        var storeResponse = await storeGetResponse.Content.ReadFromJsonAsync<Helpers.Envelope<QueryStoreDto>>();
        Assert.NotNull(storeResponse);
        Assert.Null(storeResponse.Result);
        Assert.NotNull(storeResponse.ErrorMessage);
        Assert.NotEmpty(storeResponse.ErrorMessage);
        _output.WriteLine($"Error: {storeResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteAllStores_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingChainId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/stores/deleteAllStores/{nonExistingChainId}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task DeleteAllStores_WithChainHavingNoStores_ReturnsOk() // No stores to delete. Should it still return OK? or BadRequest?
    {
        // Arrange
        var chainId = await ApiHelper.CreateChainAndGetId(_client, StringRandom.GetRandomString(10));
        _output.WriteLine($"Created Chain ID: {chainId}");

        // Act
        var response = await _client.DeleteAsync($"/api/stores/deleteAllStores/{chainId}");
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
        var task1 = _client.PostAsJsonAsync("/api/stores/postStore", request1);
        var task2 = _client.PostAsJsonAsync("/api/stores/postStore", request2);
        var responses = await Task.WhenAll(task1, task2);
        foreach (var response in responses)// log full server error for debugging
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _output.WriteLine(responseBody);
        }

        // Assert
        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));

        var store1Response = await responses[0].Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        var store2Response = await responses[1].Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(store1Response?.Result);
        Assert.NotNull(store2Response?.Result);
        Assert.Equal(101, store1Response.Result.Number);
        Assert.Equal(102, store2Response.Result.Number);
        Assert.Equal(chainId, store1Response.Result.ChainId);
        Assert.Equal(chainId, store2Response.Result.ChainId);
    }

    [Fact]
    public async Task UpdateStore_AfterDelete_ReturnsBadRequest()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var storeId = await ApiHelper.CreateStoreAndGetId(_client, storeDto);

        // Delete the store
        await _client.DeleteAsync($"/api/stores/deleteStore/{storeId}");

        // Attempt to update
        var updateRequest = new UpdateStoreDto(storeId, null, 401, "Updated Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "updated@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        var response = await _client.PutAsJsonAsync("/api/stores/putStore", updateRequest);
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var errorResponse = await response.Content.ReadFromJsonAsync<Helpers.Envelope<StoreResponseDto>>();
        Assert.NotNull(errorResponse);
        Assert.Null(errorResponse.Result);
        Assert.NotNull(errorResponse.ErrorMessage);
        Assert.NotEmpty(errorResponse.ErrorMessage);
        _output.WriteLine($"Error: {errorResponse.ErrorMessage}");
    }

    [Fact]
    public async Task GetStore_AfterSuccessfulUpdate_ReturnsUpdatedData()
    {
        // Arrange
        var storeDto = new CreateStoreDto(null, 101, "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "test@store.com", "John", "Doe");
        var store = await ApiHelper.CreateStore(_client, storeDto);
        _output.WriteLine(store.Result.Id.ToString());

        var updatedName = "Newly Updated Store";
        var updatedEmail = "new@store.com";
        var updatedStreet = "456 Updated St";
        var updatedPostalCode = "54321";
        var updatedCity = "Updated City";
        var updatedFirstName = "Jane";
        var updatedLastName = "Smith";
        var updateRequest = new UpdateStoreDto(
            store.Result.Id, 
            null, 
            203, 
            updatedName,
            updatedStreet,
            updatedPostalCode,
            updatedCity,
            "1", 
            "5559876543",
            updatedEmail,
            updatedFirstName,
            updatedLastName,
            store.Result.CreatedOn, 
            store.Result.ModifiedOn);

        // Act
        var updateResponse = await _client.PutAsJsonAsync("/api/stores/putStore", updateRequest);
        // Check that update succeeded
        if (!updateResponse.IsSuccessStatusCode)
        {
            var errorBody = await updateResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"Update failed: {errorBody}");
        }
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var response = await _client.GetAsync($"/api/stores/getStore/{store.Result.Id}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var queryStoreDto = await response.Content.ReadFromJsonAsync<Helpers.Envelope<QueryStoreDto>>();
        Assert.NotNull(queryStoreDto);
        Assert.NotNull(queryStoreDto.Result);
        Assert.Equal(store.Result.Id, queryStoreDto.Result.Id);
        Assert.Equal(updatedName, queryStoreDto.Result.Name);
        Assert.Equal(updatedEmail, queryStoreDto.Result.Email);
        Assert.Equal(203, queryStoreDto.Result.Number);
        Assert.Equal(updatedStreet, queryStoreDto.Result.Street);
        Assert.Equal(updatedPostalCode, queryStoreDto.Result.PostalCode);
        Assert.Equal(updatedCity, queryStoreDto.Result.City);
        Assert.Equal(updatedFirstName, queryStoreDto.Result.FirstName);
        Assert.Equal(updatedLastName, queryStoreDto.Result.LastName);
    }
}
