using StoreManager.Webapp.Client.Models.Chain.Command;
using StoreManager.Webapp.Client.Models.Chain.Query;

namespace StoreManager.Webapp.Client.Services;

public interface IChainService
{
    Task<ChainResponseDto> PostChainAsync(CreateChainDto request);
    Task<QueryChainDto?> GetChainByIdAsync(Guid id);
    Task<List<QueryChainDto>> GetAllChainsAsync();
    Task<QueryChainDto> GetChainAndStores(Guid id);
    Task<ChainResponseDto> PutChainAsync(UpdateChainDto request);
    Task DeleteChainAsync(Guid chainId);
}
