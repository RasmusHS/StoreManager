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

    public async Task<StoreResponseDto> CreateStoreAsync(CreateStoreDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/store/createStore", request);
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<StoreResponseDto>>();
        return envelope?.Result ?? throw new Exception("Failed to create store");
    }

    public async Task<List<StoreResponseDto>> BulkCreateStoresAsync(List<CreateStoreDto> requests)
    {
        var response = await _httpClient.PostAsJsonAsync("api/store/bulkCreateStores", requests);
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<List<StoreResponseDto>>>();
        return envelope?.Result ?? throw new Exception("Failed to bulk create stores");
    }

    public async Task<QueryStoreDto?> GetStoreByIdAsync(Guid storeId)
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<QueryStoreDto>>($"api/store/getStore/{storeId}");
        return envelope?.Result;
    }

    public async Task<List<QueryStoreDto>> GetAllIndependentStoresAsync()
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<CollectionResponseBase<QueryStoreDto>>>($"api/store/getStoresByChain/{Guid.Empty}");
        return envelope?.Result?.Data?.ToList() ?? new List<QueryStoreDto>();
    }

    public async Task<List<QueryStoreDto>> GetStoresByChainAsync(Guid chainId)
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<CollectionResponseBase<QueryStoreDto>>>($"api/store/getStoresByChain/{chainId}");
        return envelope?.Result?.Data?.ToList() ?? new List<QueryStoreDto>();
    }

    public async Task<StoreResponseDto> UpdateStoreAsync(UpdateStoreDto request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/store/updateStore", request);
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<StoreResponseDto>>();
        return envelope?.Result ?? throw new Exception("Failed to update store");
    }

    public async Task DeleteStoreAsync(DeleteStoreDto request)
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/store/deleteStore", UriKind.Relative),
            Content = JsonContent.Create(request)
        });
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAllStoresAsync(DeleteAllStoresDto request)
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/store/deleteAllStores", UriKind.Relative),
            Content = JsonContent.Create(request)
        });
        response.EnsureSuccessStatusCode();
    }
}
