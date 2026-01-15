using Xunit.Abstractions;
using System.Net;
using StoreManager.Application.DTO.Store.Command;
using System.Net.Http.Json;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Domain.Chain;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Application.DTO.Store.Query;

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
        var chainId = ChainId.Create().Value.Value;
        var request = new CreateStoreDto
        {
            ChainId = chainId,
            Number = 101,
            Name = "Test Store",
            Street = "123 Main St",
            PostalCode = "12345",
            City = "Test City",
            CountryCode = "+1",
            PhoneNumber = "5551234567",
            Email = "test@store.com",
            FirstName = "John",
            LastName = "Doe"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/store/createStore", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateStore_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var chainId = ChainId.Create().Value.Value;
        var request = new CreateStoreDto
        {
            ChainId = chainId,
            Number = 103,
            Name = "Test Store",
            Street = "123 Main St",
            PostalCode = "12345",
            City = "Test City",
            CountryCode = "+1",
            PhoneNumber = "5551234567",
            Email = "invalid-email",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/store/createStore", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateStore_WithInvalidPhoneNumber_ReturnsBadRequest()
    {
        // Arrange
        var chainId = ChainId.Create().Value.Value;
        var request = new CreateStoreDto
        {
            ChainId = chainId,
            Number = 104,
            Name = "Test Store",
            Street = "123 Main St",
            PostalCode = "12345",
            City = "Test City",
            CountryCode = "+1",
            PhoneNumber = "abc",
            Email = "test@store.com",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/store/createStore", request);

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

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStore_WithExistingStoreId_ReturnsOkWithStore()
    {
        // Arrange
        var storeId = StoreId.Create().Value.Value;

        // Act
        var response = await _client.GetAsync($"/api/store/getStore/{storeId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetStore_WithNonExistingStoreId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/store/getStore/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithExistingChain_ReturnsOkWithStores()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain");
        var store = new List<StoreEntity>
        {
            StoreEntity.Create(chain.Value.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("+1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value,
            StoreEntity.Create(chain.Value.Id, 2, "Test Store 2", Address.Create("456 Test St", "67890", "Test City"), PhoneNumber.Create("+1", "5559876543"), Email.Create("store2@test.com"), FullName.Create("Test", "Manager")).Value
        };
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain.Value);
        await DbContext.StoreEntities.AddRangeAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();

        // Act
        var response = await _client.GetFromJsonAsync<List<QueryStoreDto>>($"/api/store/getStoresByChain/{chain.Value.Id.Value}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task GetStoresByChainId_WithExistingChainId_ReturnsOkWithOneStore()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("+1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();

        // Act
        var response = await _client.GetFromJsonAsync<QueryStoreDto>($"/api/store/getStoresByChain/{chain.Id.Value}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(store.Id.Value, response.Id);
    }

    [Fact]
    public async Task GetStoresByChainId_WithNonExistingChainId_ReturnsKeyNotFoundException()
    {
        // Arrange
        var chain = ChainId.GetExisting(Guid.NewGuid()).Value;
        var store = new List<StoreEntity>
        {
            StoreEntity.Create(chain, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("+1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value,
            StoreEntity.Create(chain, 2, "Test Store 2", Address.Create("456 Test St", "67890", "Test City"), PhoneNumber.Create("+1", "5559876543"), Email.Create("store2@test.com"), FullName.Create("Test", "Manager")).Value
        };
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.StoreEntities.AddRangeAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();

        // Act
        var response = await _client.GetFromJsonAsync<List<QueryStoreDto>>($"/api/store/getStoresByChain/{chain.Value.Id.Value}");

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task GetStoresByChainId_WithChainHavingNoStores_ReturnsEmptyList()
    {
        // Arrange
        var chainId = await CreateTestChain();

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chainId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithNonExistingChainId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{nonExistingId}");

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var chainId = await CreateTestChain();
        var storeId = await CreateTestStoreForChain(chainId);
        var request = new UpdateStoreDto
        {
            Id = storeId,
            ChainId = chainId,
            Number = 201,
            Name = "Updated Store",
            Street = "456 Updated St",
            PostalCode = "54321",
            City = "Updated City",
            CountryCode = "+1",
            PhoneNumber = "5559876543",
            Email = "updated@store.com",
            FirstName = "Jane",
            LastName = "Smith",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            ModifiedOn = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var chainId = await CreateTestChain();
        var request = new UpdateStoreDto
        {
            Id = Guid.NewGuid(),
            ChainId = chainId,
            Number = 202,
            Name = "Non-existing Store",
            Street = "456 Updated St",
            PostalCode = "54321",
            City = "Updated City",
            CountryCode = "+1",
            PhoneNumber = "5559876543",
            Email = "test@store.com",
            FirstName = "Jane",
            LastName = "Smith",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            ModifiedOn = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithInvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var chainId = await CreateTestChain();
        var storeId = await CreateTestStoreForChain(chainId);
        var request = new UpdateStoreDto
        {
            Id = storeId,
            ChainId = chainId,
            Number = 203,
            Name = "Updated Store",
            Street = "456 Updated St",
            PostalCode = "54321",
            City = "Updated City",
            CountryCode = "+1",
            PhoneNumber = "5559876543",
            Email = "invalid-email",
            FirstName = "Jane",
            LastName = "Smith",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            ModifiedOn = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", request);

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

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStore_WithExistingStoreId_ReturnsOkResult()
    {
        // Arrange
        var storeId = await CreateTestStore();
        var request = new DeleteStoreDto { Id = storeId };

        // Act
        var response = await _client.DeleteAsync($"/api/store/deleteStore");
        response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(request)
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeleteStoreDto { Id = Guid.NewGuid() };

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(request)
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStore_WithInvalidDto_ReturnsBadRequestWithValidationErrors()
    {
        // Arrange
        var request = new DeleteStoreDto { Id = Guid.Empty };

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(request)
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllStores_WithExistingChainId_DeletesAllStoresForChain()
    {
        // Arrange
        var chainId = await CreateTestChain();
        await CreateTestStoreForChain(chainId);
        await CreateTestStoreForChain(chainId);
        var request = new DeleteStoreDto { ChainId = chainId };

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteAllStores")
        {
            Content = JsonContent.Create(request)
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllStores_WithNonExistingChainId_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeleteStoreDto { ChainId = Guid.NewGuid() };

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteAllStores")
        {
            Content = JsonContent.Create(request)
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteAllStores_WithChainHavingNoStores_ReturnsOk()
    {
        // Arrange
        var chainId = await CreateTestChain();
        var request = new DeleteStoreDto { ChainId = chainId };

        // Act
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteAllStores")
        {
            Content = JsonContent.Create(request)
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateStore_ConcurrentRequests_BothSucceed()
    {
        // Arrange
        var chainId = await CreateTestChain();
        var request1 = CreateValidStoreDto(chainId, 301);
        var request2 = CreateValidStoreDto(chainId, 302);

        // Act
        var task1 = _client.PostAsJsonAsync("/api/store/createStore", request1);
        var task2 = _client.PostAsJsonAsync("/api/store/createStore", request2);
        var responses = await Task.WhenAll(task1, task2);

        // Assert
        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));
    }

    [Fact]
    public async Task UpdateStore_AfterDelete_ReturnsBadRequest()
    {
        // Arrange
        var chainId = await CreateTestChain();
        var storeId = await CreateTestStoreForChain(chainId);
        
        // Delete the store
        var deleteRequest = new DeleteStoreDto { Id = storeId };
        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(deleteRequest)
        });

        // Attempt to update
        var updateRequest = new UpdateStoreDto
        {
            Id = storeId,
            ChainId = chainId,
            Number = 401,
            Name = "Updated Store",
            Street = "456 Updated St",
            PostalCode = "54321",
            City = "Updated City",
            CountryCode = "+1",
            PhoneNumber = "5559876543",
            Email = "updated@store.com",
            FirstName = "Jane",
            LastName = "Smith",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            ModifiedOn = DateTime.UtcNow
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/store/updateStore", updateRequest);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetStore_AfterSuccessfulUpdate_ReturnsUpdatedData()
    {
        // Arrange
        var chainId = await CreateTestChain();
        var storeId = await CreateTestStoreForChain(chainId);
        var updatedName = "Newly Updated Store";
        var updateRequest = new UpdateStoreDto
        {
            Id = storeId,
            ChainId = chainId,
            Number = 501,
            Name = updatedName,
            Street = "789 New St",
            PostalCode = "99999",
            City = "New City",
            CountryCode = "+1",
            PhoneNumber = "5551111111",
            Email = "new@store.com",
            FirstName = "Updated",
            LastName = "Manager",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            ModifiedOn = DateTime.UtcNow
        };

        // Act
        await _client.PutAsJsonAsync("/api/store/updateStore", updateRequest);
        var response = await _client.GetAsync($"/api/store/getStore/{storeId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(updatedName, content);
    }
}
