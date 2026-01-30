using StoreManager.Webapp.Client.Models.Store.Command;
using StoreManager.Webapp.Client.Models.Store.Query;

namespace StoreManager.Webapp.Client.Services;

public interface IStoreService
{
    Task<StoreResponseDto> PostStoreAsync(CreateStoreDto request);
    Task<List<StoreResponseDto>> PostBulkStoresAsync(List<CreateStoreDto> requests);
    Task<QueryStoreDto?> GetStoreByIdAsync(Guid storeId);
    Task<List<QueryStoreDto>> GetAllIndependentStoresAsync();
    Task<List<QueryStoreDto>> GetStoresByChainAsync(Guid chainId);
    Task<StoreResponseDto> PutStoreAsync(UpdateStoreDto request);
    Task DeleteStoreAsync(Guid storeId);
    Task DeleteAllStoresAsync(Guid chainId);
}
