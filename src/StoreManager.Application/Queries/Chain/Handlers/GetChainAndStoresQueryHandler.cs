using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Chain.Query;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Queries.Chain.Handlers;

public class GetChainAndStoresQueryHandler : IQueryHandler<GetChainAndStoresQuery, QueryChainDto>
{
    private readonly IChainRepository _chainRepository;
    
    public GetChainAndStoresQueryHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result<QueryChainDto>> Handle(GetChainAndStoresQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var chainResult = await _chainRepository.GetByIdIncludeStoresAsync(query.Id);
            if (chainResult == null)
            {
                return Result.Fail<QueryChainDto>(Errors.General.NotFound<ChainId>(query.Id));
            }
            if (chainResult.Stores.Count() < 1)
            {
                return Result.Fail<QueryChainDto>(Errors.ChainErrors.ChainHasNoStores<ChainId>(query.Id));
            }
            var chainDto = new QueryChainDto
            {
                Id = chainResult.Id.Value,
                Name = chainResult.Name,
                Stores = chainResult.Stores!.Select(s => new QueryStoreDto(
                    s.Id.Value,
                    s.ChainId!.Value,
                    s.Number,
                    s.Name,
                    s.Address.Street,
                    s.Address.PostalCode,
                    s.Address.City,
                    s.PhoneNumber.CountryCode,
                    s.PhoneNumber.Number,
                    s.Email.Value,
                    s.StoreOwner.FirstName,
                    s.StoreOwner.LastName,
                    s.CreatedOn,
                    s.ModifiedOn)).ToList(),
                CreatedOn = chainResult.CreatedOn,
                ModifiedOn = chainResult.ModifiedOn
            };
            return Result.Ok(chainDto);
        }
        catch (Exception ex)
        {
            return Result.Fail<QueryChainDto>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
