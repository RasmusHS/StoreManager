using StoreManager.Application.Data;
using StoreManager.Application.Data.Infrastructure;
using StoreManager.Application.DTO.Chain.Query;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;

namespace StoreManager.Application.Queries.Chain.Handlers;

public class GetChainQueryHandler : IQueryHandler<GetChainQuery, QueryChainDto>
{
    private readonly IChainRepository _chainRepository;
    
    public GetChainQueryHandler(IChainRepository chainRepository)
    {
        _chainRepository = chainRepository;
    }

    public async Task<Result<QueryChainDto>> Handle(GetChainQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var chainResult = await _chainRepository.GetByIdAsync(query.Id);
            if (chainResult == null)
            {
                return Result.Fail<QueryChainDto>(Errors.General.NotFound<ChainId>(query.Id));
            }
            int storeCount = await _chainRepository.GetCountofStoresByChainAsync(query.Id);

            var chainDto = new QueryChainDto(
                chainResult.Id.Value,
                chainResult.Name,
                null,
                storeCount,
                chainResult.CreatedOn,
                chainResult.ModifiedOn
                );
            return Result.Ok(chainDto); 
        }
        catch (Exception ex)
        {
            return Result.Fail<QueryChainDto>(Errors.General.ExceptionThrown(ex.Message));
        }
    }
}
