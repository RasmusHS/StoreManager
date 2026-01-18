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
        return await response.Content.ReadFromJsonAsync<StoreResponseDto>()
            ?? throw new Exception("Failed to create store");
    }

    public async Task<QueryStoreDto?> GetStoreByIdAsync(Guid storeId)
    {
        return await _httpClient.GetFromJsonAsync<QueryStoreDto>($"api/store/getStore/{storeId}");
    }

    public async Task<List<QueryStoreDto>> GetAllIndependentStoresAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<QueryStoreDto>>($"api/store/getStoresByChain/{Guid.Empty}");
        return response ?? new List<QueryStoreDto>();
    }

    public async Task<List<QueryStoreDto>> GetStoresByChainAsync(Guid chainId)
    {
        var response = await _httpClient.GetFromJsonAsync<List<QueryStoreDto>>($"api/store/getStoresByChain/{chainId}");
        return response ?? new List<QueryStoreDto>();
    }

    public async Task<StoreResponseDto> UpdateStoreAsync(UpdateStoreDto request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/store/updateStore", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<StoreResponseDto>()
            ?? throw new Exception("Failed to update store");
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
