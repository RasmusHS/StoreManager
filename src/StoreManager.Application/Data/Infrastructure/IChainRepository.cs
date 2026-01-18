using StoreManager.Domain.Chain;
using StoreManager.Domain.Store;

namespace StoreManager.Application.Data.Infrastructure;

public interface IChainRepository : IAsyncRepository<ChainEntity>
{
    Task<ChainEntity> GetByIdIncludeStoresAsync(object? id);
    Task<int> GetCountofStoresByChainAsync(object? id);
    Task<IReadOnlyList<ChainEntity>> GetAllChainsAsync();
}
