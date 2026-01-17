using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Queries.Store.Handlers;

public class GetStoreQueryHandler : IQueryHandler<GetStoreQuery, QueryStoreDto>
{
    private readonly IStoreRepository _storeRepository;

    public GetStoreQueryHandler(IStoreRepository storeRepository)
    {
        _storeRepository = storeRepository;
    }

    public async Task<Result<QueryStoreDto>> Handle(GetStoreQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var storeResult = await _storeRepository.GetByIdAsync(query.Id);
            if (storeResult == null)
            {
                return Result.Fail<QueryStoreDto>(Errors.General.NotFound<StoreId>(query.Id));
            }

            var storeDto = new QueryStoreDto(
                storeResult.Id.Value,
                storeResult.ChainId?.Value,
                storeResult.Number,
                storeResult.Name,
                storeResult.Address.Street,
                storeResult.Address.PostalCode,
                storeResult.Address.City,
                storeResult.PhoneNumber.CountryCode,
                storeResult.PhoneNumber.Number,
                storeResult.Email.Value,
                storeResult.StoreOwner.FirstName,
                storeResult.StoreOwner.LastName,
                storeResult.CreatedOn,
                storeResult.ModifiedOn);

            return Result.Ok(storeDto); 
        }
        catch (Exception ex)
        {
            return Result.Fail<QueryStoreDto>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
