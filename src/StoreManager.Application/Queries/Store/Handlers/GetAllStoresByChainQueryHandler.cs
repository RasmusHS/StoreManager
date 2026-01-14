using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;

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
        List<QueryStoreDto> stores = new List<QueryStoreDto>();
        var storesResult = await _storeRepository.GetAllByChainIdAsync(query.ChainId) ?? throw new KeyNotFoundException($"Stores belonging to Chain with ID {query.ChainId} not found.");
        if (storesResult.Count() < 1)
        {
            throw new KeyNotFoundException($"Stores belonging to Chain with ID {query.ChainId} not found.");
        }

        foreach (var store in storesResult)
        {
            QueryStoreDto storeDto = new QueryStoreDto(
                store.Id.Value,
                store.ChainId!.Value,
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
}
