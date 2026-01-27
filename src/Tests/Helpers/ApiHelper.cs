using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Application.DTO.Store.Command;
using System.Net.Http.Json;
using System.Text.Json;

namespace Helpers;

public static class ApiHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true // Handle both PascalCase and camelCase
    };

    public static async Task<Guid> CreateChainAndGetId(HttpClient client, string chainName)
    {
        var dto = new CreateChainDto(chainName);
        var response = await client.PostAsJsonAsync("/api/chains/postChain", dto);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        // DEBUG: Print the actual response structure
        //Console.WriteLine($"==== DEBUG: Chain API Response ====");
        //Console.WriteLine(responseBody);
        //Console.WriteLine($"====================================");

        var chainResponse = JsonSerializer.Deserialize<ChainResponseDto>(responseBody, JsonOptions);

        //Console.WriteLine($"DEBUG: Deserialized ChainId = {chainResponse?.ChainId}");

        if (chainResponse?.ChainId == Guid.Empty || chainResponse == null)
        {
            throw new InvalidOperationException($"Failed to extract ChainId. Response: {responseBody}");
        }

        return chainResponse.ChainId;
    }

    public static async Task<Guid> CreateStoreAndGetId(HttpClient client, CreateStoreDto dto)
    {
        var response = await client.PostAsJsonAsync("/api/stores/postStore", dto);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        // DEBUG: Print the actual response structure
        //Console.WriteLine($"==== DEBUG: Store API Response ====");
        //Console.WriteLine(responseBody);
        //Console.WriteLine($"====================================");

        var storeResponse = JsonSerializer.Deserialize<StoreResponseDto>(responseBody, JsonOptions);

        //Console.WriteLine($"DEBUG: Deserialized StoreId = {storeResponse?.Id}");

        if (storeResponse?.Id == Guid.Empty || storeResponse == null)
        {
            throw new InvalidOperationException($"Failed to extract StoreId. Response: {responseBody}");
        }

        return storeResponse.Id;
    }

    public static async Task<ChainResponseDto> CreateChain(HttpClient client, string chainName)
    {
        var dto = new CreateChainDto(chainName);
        var response = await client.PostAsJsonAsync("/api/chains/postChain", dto);
        response.EnsureSuccessStatusCode();

        var chainResponse = await response.Content.ReadFromJsonAsync<ChainResponseDto>(JsonOptions);
        return chainResponse!;
    }

    public static async Task<StoreResponseDto> CreateStore(HttpClient client, CreateStoreDto dto)
    {
        var response = await client.PostAsJsonAsync("/api/stores/postStore", dto);
        response.EnsureSuccessStatusCode();

        var storeResponse = await response.Content.ReadFromJsonAsync<StoreResponseDto>(JsonOptions);
        return storeResponse!;
    }
}
