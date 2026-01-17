using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Queries.Store.Handlers;

public class GetAllStoresByChainQueryHandler : IQueryHandler<GetAllStoresByChainQuery, CollectionResponseBase<QueryStoreDto>>
{
    private readonly IStoreRepository _storeRepository;
    private readonly IChainRepository _chainRepository;

    public GetAllStoresByChainQueryHandler(IStoreRepository storeRepository, IChainRepository chainRepository)
    {
        _storeRepository = storeRepository;
        _chainRepository = chainRepository;
    }

    public async Task<Result<CollectionResponseBase<QueryStoreDto>>> Handle(GetAllStoresByChainQuery query, CancellationToken cancellationToken = default)
    {
        if (await _chainRepository.GetByIdAsync(query.ChainId) == null)
        {
            return Result.Fail<CollectionResponseBase<QueryStoreDto>>(Errors.General.NotFound<ChainId>(query.ChainId));
        }

        List<QueryStoreDto> stores = new List<QueryStoreDto>();
        var storesResult = await _storeRepository.GetAllByChainIdAsync(query.ChainId);
        if (storesResult.Count() < 1)
        {
            return Result.Fail<CollectionResponseBase<QueryStoreDto>>(Errors.ChainErrors.ChainHasNoStores<ChainId>(query.ChainId));
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
