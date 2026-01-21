using StoreManager.Webapp.Client.Models.Store.Command;
using StoreManager.Webapp.Client.Models.Store.Query;

namespace StoreManager.Webapp.Client.Services;

public interface IStoreService
{
    Task<StoreResponseDto> CreateStoreAsync(CreateStoreDto request);
    Task<List<StoreResponseDto>> BulkCreateStoresAsync(List<CreateStoreDto> requests);
    Task<QueryStoreDto?> GetStoreByIdAsync(Guid storeId);
    Task<List<QueryStoreDto>> GetAllIndependentStoresAsync();
    Task<List<QueryStoreDto>> GetStoresByChainAsync(Guid chainId);
    Task<StoreResponseDto> UpdateStoreAsync(UpdateStoreDto request);
    Task DeleteStoreAsync(DeleteStoreDto request);
    Task DeleteAllStoresAsync(DeleteAllStoresDto request);
}
