using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Chain.Query;
using StoreManager.Application.DTO.Store.Query;
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
        //List<QueryStoreDto> stores = new List<QueryStoreDto>();
        var chainResult = await _chainRepository.GetByIdIncludeStoresAsync(query.Id) ?? throw new KeyNotFoundException($"Stores belonging to Chain with ID {query.Id} not found.");
        if (chainResult.Stores.Count() < 1)
        {
            throw new KeyNotFoundException($"Stores belonging to Chain with ID {query.Id} not found.");
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
}
