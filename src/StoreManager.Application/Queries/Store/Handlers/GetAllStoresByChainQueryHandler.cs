using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Queries.Store.Handlers;

public class GetAllStoresByChainQueryHandler : IQueryHandler<GetAllStoresByChainQuery, CollectionResponseBase<QueryStoreDto>>
{
    private readonly IStoreRepository _storeRepository;

    public GetAllStoresByChainQueryHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result<CollectionResponseBase<QueryStoreDto>>> Handle(GetAllStoresByChainQuery query, CancellationToken cancellationToken = default)
    {
        try 
        {
            List<QueryStoreDto> stores = new List<QueryStoreDto>();

            // Get stores - either for a specific chain or independent stores (no chain)
            var storesResult = (query.ChainId?.Value == Guid.Empty || query.ChainId == null)
                ? await _storeRepository.GetAllIndependentStoresAsync()  // New method for stores where chainId IS NULL
                : await _storeRepository.GetAllByChainIdAsync(query.ChainId);

            if (storesResult.Count() < 1)
            {
                return Result.Fail<CollectionResponseBase<QueryStoreDto>>(
                    query.ChainId == null || query.ChainId.Value == Guid.Empty
                        ? Errors.StoreErrors.NoIndependentStoresFound()  // Or appropriate error
                        : Errors.ChainErrors.ChainHasNoStores<ChainId>(query.ChainId));
            }

            foreach (var store in storesResult)
            {
                QueryStoreDto storeDto = new QueryStoreDto(
                    store.Id.Value,
                    store.ChainId?.Value ?? null,
                    store.Number,
                    store.Name,
                    store.Address.Street,
                    store.Address.PostalCode,
                    store.Address.City,
                    store.PhoneNumber.CountryCode,
                    store.PhoneNumber.Number,
                    store.Email.Value,
                    store.StoreOwner.FirstName,
                    store.StoreOwner.LastName,
                    store.CreatedOn,
                    store.ModifiedOn
                    );
                stores.Add(storeDto);
            }
            return new CollectionResponseBase<QueryStoreDto>()
            {
                Data = stores
            };
        }
        catch (Exception ex)
        {
            return Result.Fail<CollectionResponseBase<QueryStoreDto>>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
