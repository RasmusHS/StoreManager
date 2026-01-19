using StoreManager.Webapp.Client.Models.Chain.Command;
using StoreManager.Webapp.Client.Models.Chain.Query;
using StoreManager.Webapp.Client.Models;
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
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<ChainResponseDto>>();
        return envelope?.Result ?? throw new Exception("Failed to create store");
    }

    public async Task<QueryChainDto?> GetChainByIdAsync(Guid id)
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<QueryChainDto>>($"api/chain/getChain/{id}");
        return envelope?.Result;
    }

    public async Task<List<QueryChainDto>> GetAllChainsAsync()
    {
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<CollectionResponseBase<QueryChainDto>>>("api/chain/getAllChains");
        return envelope?.Result?.Data?.ToList() ?? new List<QueryChainDto>();
    }

    public async Task<List<QueryChainDto>> GetByIdIncludeStoresAsync(Guid id)
    {
        // API returns a single chain with stores, not a collection
        var envelope = await _httpClient.GetFromJsonAsync<Envelope<QueryChainDto>>($"api/chain/getChainAndStores/{id}");

        if (envelope?.Result == null)
            return new List<QueryChainDto>();

        // Return a list with the single chain that includes stores
        return new List<QueryChainDto> { envelope.Result };
    }

    public async Task<ChainResponseDto> UpdateChainAsync(UpdateChainDto request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/chain/updateChain", request);
        response.EnsureSuccessStatusCode();
        var envelope = await response.Content.ReadFromJsonAsync<Envelope<ChainResponseDto>>();
        return envelope?.Result ?? throw new Exception("Failed to update chain");
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
