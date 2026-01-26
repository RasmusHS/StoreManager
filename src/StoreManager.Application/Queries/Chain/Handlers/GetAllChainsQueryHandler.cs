using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Chain.Query;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;

namespace StoreManager.Application.Queries.Chain.Handlers;

public class GetAllChainsQueryHandler : IQueryHandler<GetAllChainsQuery, CollectionResponseBase<QueryChainDto>>
{
    private readonly IChainRepository _chainRepository;

    public GetAllChainsQueryHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result<CollectionResponseBase<QueryChainDto>>> Handle(GetAllChainsQuery query, CancellationToken cancellationToken = default)
    {
        List<QueryChainDto> result = new List<QueryChainDto>();
        List<Error> errors = new List<Error>();

        try
        {
            var chains = await _chainRepository.GetAllChainsAsync();
            if (chains == null || !chains.Any())
            {
                return Result.Fail<CollectionResponseBase<QueryChainDto>>(Errors.ChainErrors.NoChainsExist());
            }
            foreach (var chain in chains)
            {
                QueryChainDto dto = new QueryChainDto(
                    chain.Id.Value,
                    chain.Name,
                    null,
                    await _chainRepository.GetCountofStoresByChainAsync(chain.Id), // Potential optimization: Create a StoreCount property in Chain entity with a trigger in the database to maintain the count
                    chain.CreatedOn,
                    chain.ModifiedOn);
                result.Add(dto);
            }
            return new CollectionResponseBase<QueryChainDto>()
            {
                Data = result
            };
        }
        catch (Exception ex)
        {
            return Result.Fail<CollectionResponseBase<QueryChainDto>>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
