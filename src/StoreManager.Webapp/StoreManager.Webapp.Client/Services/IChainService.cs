using StoreManager.Webapp.Client.Models.Chain.Command;
using StoreManager.Webapp.Client.Models.Chain.Query;

namespace StoreManager.Webapp.Client.Services;

public interface IChainService
{
    Task<ChainResponseDto> CreateChainAsync(CreateChainDto request);
    Task<QueryChainDto?> GetChainByIdAsync(Guid id);
    Task<List<QueryChainDto>> GetAllChainsAsync();
    Task<List<QueryChainDto>> GetByIdIncludeStoresAsync(Guid id);
    Task<ChainResponseDto> UpdateChainAsync(UpdateChainDto request);
    Task DeleteChainAsync(DeleteChainDto request);
}
