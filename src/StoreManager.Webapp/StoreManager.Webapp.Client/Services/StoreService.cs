using StoreManager.Webapp.Client.Models;
using StoreManager.Webapp.Client.Models.Store.Command;
using StoreManager.Webapp.Client.Models.Store.Query;
using System.Net.Http.Json;

namespace StoreManager.Webapp.Client.Services;

public class StoreService : IStoreService
{
    private readonly HttpClient _httpClient;

    public StoreService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<StoreResponseDto> PostStoreAsync(CreateStoreDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/stores/postStore", request);
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<StoreResponseDto>>();
        return envelope?.Result ?? throw new Exception("Failed to create store");
    }

    public async Task<List<StoreResponseDto>> PostBulkStoresAsync(List<CreateStoreDto> requests)
    {
        var response = await _httpClient.PostAsJsonAsync("api/stores/postBulkStores", requests);
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<List<StoreResponseDto>>>();
        return envelope?.Result ?? throw new Exception("Failed to bulk create stores");
    }

    public async Task<QueryStoreDto?> GetStoreByIdAsync(Guid storeId)
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<QueryStoreDto>>($"api/stores/getStore/{storeId}");
        return envelope?.Result;
    }

    public async Task<List<QueryStoreDto>> GetAllIndependentStoresAsync()
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<CollectionResponseBase<QueryStoreDto>>>($"api/stores/getStoresByChain/{Guid.Empty}");
        return envelope?.Result?.Data?.ToList() ?? new List<QueryStoreDto>();
    }

    public async Task<List<QueryStoreDto>> GetStoresByChainAsync(Guid chainId)
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<CollectionResponseBase<QueryStoreDto>>>($"api/stores/getStoresByChain/{chainId}");
        return envelope?.Result?.Data?.ToList() ?? new List<QueryStoreDto>();
    }

    public async Task<StoreResponseDto> PutStoreAsync(UpdateStoreDto request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/stores/updateStore", request);
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<StoreResponseDto>>();
        return envelope?.Result ?? throw new Exception("Failed to update store");
    }

    public async Task DeleteStoreAsync(Guid storeId)
    {
        await _httpClient.DeleteAsync($"api/stores/deleteStore/{storeId}");
    }

    public async Task DeleteAllStoresAsync(Guid chainId)
    {
        await _httpClient.DeleteAsync($"api/stores/deleteAllStores/{chainId}");
    }
}
