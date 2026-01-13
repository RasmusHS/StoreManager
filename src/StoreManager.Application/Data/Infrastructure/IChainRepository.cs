using StoreManager.Domain.Chain;

namespace StoreManager.Application.Data.Infrastructure;

public interface IChainRepository : IAsyncRepository<ChainEntity>
{
    Task<ChainEntity> GetByIdIncludeStoresAsync(object id);
}
