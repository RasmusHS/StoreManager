using StoreManager.Domain.Store;

namespace StoreManager.Application.Data.Infrastructure;

public interface IStoreRepository : IAsyncRepository<StoreEntity>
{
    Task<IReadOnlyList<StoreEntity>> GetAllByChainIdAsync(object chainId);
}
