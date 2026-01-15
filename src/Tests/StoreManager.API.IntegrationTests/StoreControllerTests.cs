using Azure;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;
using System.Net;
using System.Net.Http.Json;
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

    [Theory]
    [InlineData(103 , "Test Store", "123 Main St", "12345", "Test City", "1", "5551234567", "invalid-mail.com", "John", "Doe")]
    [InlineData(103, "Test Store", "123 Main St", "12345", "Test City", "1", "abc", "test@store.com", "John", "Doe")]
    public async Task CreateStore_WithInvalidInput_ReturnsBadRequest(int number, string name, string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName)
    {
        // Arrange
        var request = new CreateStoreDto
        {
            ChainId = null,
            Number = number,
            Name = name,
            Street = street,
            PostalCode = postalCode,
            City = city,
            CountryCode = countryCode,
            PhoneNumber = phoneNumber,
            Email = email,
            FirstName = firstName,
            LastName = lastName
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
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

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
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

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
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chain.Value.Id.Value}");
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
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("+1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chain.Id.Value}");
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
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chain.Value}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task GetStoresByChainId_WithChainHavingNoStores_ReturnsEmptyList()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();

        // Act
        var response = await _client.GetAsync($"/api/store/getStoresByChain/{chain.Id}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

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
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStore_WithValidData_ReturnsOkResult()
    {
        // Arrange
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request = new UpdateStoreDto
        {
            Id = store.Id.Value,
            ChainId = chain.Id.Value,
            Number = 201,
            Name = "Updated Store",
            Street = "456 Updated St",
            PostalCode = "54321",
            City = "Updated City",
            CountryCode = "1",
            PhoneNumber = "5559876543",
            Email = "updated@store.com",
            FirstName = "Jane",
            LastName = "Smith",
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow.AddDays(1)
        };

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
        var chain = ChainEntity.Create("Test Chain").Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request = new UpdateStoreDto
        {
            Id = Guid.NewGuid(),
            ChainId = chain.Id.Value,
            Number = 202,
            Name = "Non-existing Store",
            Street = "456 Updated St",
            PostalCode = "54321",
            City = "Updated City",
            CountryCode = "1",
            PhoneNumber = "5559876543",
            Email = "test@store.com",
            FirstName = "Jane",
            LastName = "Smith",
            CreatedOn = DateTime.UtcNow.AddDays(-1),
            ModifiedOn = DateTime.UtcNow
        };

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
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request = new UpdateStoreDto(store.Id.Value, chain.Id.Value, 203, "Updated Store", "456 Updated St", "54321", "Updated City", "1", "5559876543", "invalid-email", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

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
        var store = StoreEntity.Create(null, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request = new DeleteStoreDto(store.Id.Value, store.Chain.Id.Value, store.CreatedOn, store.ModifiedOn);

        // Act
        //var response = await _client.DeleteAsync($"/api/store/deleteStore");
        var response = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(request)
        });
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteStore_WithNonExistingStoreId_ReturnsBadRequest()
    {
        // Arrange
        var request = new DeleteStoreDto(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);

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
        var request = new DeleteStoreDto(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow);

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
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = new List<StoreEntity>
        {
            StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("+1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value,
            StoreEntity.Create(chain.Id, 2, "Test Store 2", Address.Create("456 Test St", "67890", "Test City"), PhoneNumber.Create("+1", "5559876543"), Email.Create("store2@test.com"), FullName.Create("Test", "Manager")).Value
        };
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddRangeAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request = new DeleteAllStoresDto(chain.Id.Value, chain.CreatedOn, chain.ModifiedOn);

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
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = new List<StoreEntity>
        {
            StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("+1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value,
            StoreEntity.Create(chain.Id, 2, "Test Store 2", Address.Create("456 Test St", "67890", "Test City"), PhoneNumber.Create("+1", "5559876543"), Email.Create("store2@test.com"), FullName.Create("Test", "Manager")).Value
        };
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddRangeAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request = new DeleteAllStoresDto(Guid.NewGuid(), chain.CreatedOn, chain.ModifiedOn);

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
        var chain = ChainEntity.Create("Test Chain").Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request = new DeleteAllStoresDto(chain.Id.Value, chain.CreatedOn, chain.ModifiedOn);

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
        var chain = ChainEntity.Create("Test Chain").Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var request1 = new CreateStoreDto
        {
            ChainId = chain.Id.Value,
            Number = 101,
            Name = "Test Store",
            Street = "123 Main St",
            PostalCode = "12345",
            City = "Test City",
            CountryCode = "1",
            PhoneNumber = "5551234567",
            Email = "test@store.com",
            FirstName = "John",
            LastName = "Doe"
        };
        var request2 = new CreateStoreDto
        {
            ChainId = chain.Id.Value,
            Number = 101,
            Name = "Test Store",
            Street = "123 Main St",
            PostalCode = "12345",
            City = "Test City",
            CountryCode = "1",
            PhoneNumber = "5551234567",
            Email = "test@store.com",
            FirstName = "John",
            LastName = "Doe"
        };

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
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();

        // Delete the store
        var deleteRequest = new DeleteStoreDto(store.Id.Value, store.Chain.Id.Value, store.CreatedOn, store.ModifiedOn);
        await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/api/store/deleteStore")
        {
            Content = JsonContent.Create(deleteRequest)
        });

        // Attempt to update
        var updateRequest = new UpdateStoreDto
        {
            Id = store.Id.Value,
            ChainId = chain.Id.Value,
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
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow.AddDays(1)
        };

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
        var chain = ChainEntity.Create("Test Chain").Value;
        var store = StoreEntity.Create(chain.Id, 1, "Test Store 1", Address.Create("123 Test St", "12345", "Test City"), PhoneNumber.Create("1", "5551234567"), Email.Create("store1@test.com"), FullName.Create("Test", "Manager")).Value;
        await DbContext.Database.BeginTransactionAsync();
        await DbContext.ChainEntities.AddAsync(chain);
        await DbContext.StoreEntities.AddAsync(store);
        await DbContext.SaveChangesAsync();
        await DbContext.Database.CommitTransactionAsync();
        var updatedName = "Newly Updated Store";
        var updateRequest = new UpdateStoreDto(store.Id.Value, chain.Id.Value, 203, updatedName, "456 Updated St", "54321", "Updated City", "1", "5559876543", "new@store.com", "Jane", "Smith", DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        // Act
        await _client.PutAsJsonAsync("/api/store/updateStore", updateRequest);
        var response = await _client.GetAsync($"/api/store/getStore/{store.Id.Value}");
        var responseBody = await response.Content.ReadAsStringAsync(); // log full server error for debugging
        _output.WriteLine(responseBody);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains(updatedName, content);
    }
}
