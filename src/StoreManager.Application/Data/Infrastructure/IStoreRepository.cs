using StoreManager.Domain.Store;

namespace StoreManager.Application.Data.Infrastructure;

public interface IStoreRepository : IAsyncRepository<StoreEntity>
{
    Task<IReadOnlyList<StoreEntity>> GetAllByChainIdAsync(object chainId);
    Task DeleteByChainIdAsync(object chainId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StoreEntity>> GetAllIndependentStoresAsync();
}
