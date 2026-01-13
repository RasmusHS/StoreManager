using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Domain.Common;

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
        var storeResult = await _storeRepository.GetByIdAsync(query.Id, cancellationToken) ?? throw new KeyNotFoundException($"Store with ID {query.Id} not found.");

        var storeDto = new QueryStoreDto(
            storeResult.Id,
            storeResult.ChainId,
            storeResult.Number,
            storeResult.Name,
            storeResult.Street,
            storeResult.PostalCode,
            storeResult.City,
            storeResult.CountryCode,
            storeResult.PhoneNumber,
            storeResult.Email,
            storeResult.FirstName,
            storeResult.LastName,
            storeResult.CreatedOn,
            storeResult.ModifiedOn);

        return Result.Ok(storeDto);
    }
}
