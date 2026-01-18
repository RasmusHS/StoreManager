using StoreManager.Webapp.Client.Models.Chain.Command;
using StoreManager.Webapp.Client.Models.Chain.Query;
using System.Net.Http.Json;

namespace StoreManager.Webapp.Client.Services;

public class ChainService : IChainService
{
    private readonly HttpClient _httpClient;
    
    public ChainService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ChainResponseDto> CreateChainAsync(CreateChainDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/chain/createChain", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ChainResponseDto>()
            ?? throw new Exception("Failed to create store");
    }

    public async Task<QueryChainDto?> GetChainByIdAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<QueryChainDto>($"api/chain/getChain/{id}");
    }

    public async Task<List<QueryChainDto>> GetAllChainsAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<List<QueryChainDto>>("api/chain/getAllChains");
        return response ?? new List<QueryChainDto>();
    }

    public async Task<List<QueryChainDto>> GetByIdIncludeStoresAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<List<QueryChainDto>>($"api/chain/getChainAndStores/{id}");
        return response ?? new List<QueryChainDto>();
    }

    public async Task<ChainResponseDto> UpdateChainAsync(UpdateChainDto request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/chain/updateChain", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ChainResponseDto>()
            ?? throw new Exception("Failed to update chain");
    }

    public async Task DeleteChainAsync(DeleteChainDto request)
    {
        var response = await _httpClient.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("api/chain/deleteChain", UriKind.Relative),
            Content = JsonContent.Create(request)
        });
        response.EnsureSuccessStatusCode();
    }
}
