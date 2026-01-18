using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Query;

namespace StoreManager.Application.Queries.Chain;

public record GetAllChainsQuery : IQuery<CollectionResponseBase<QueryChainDto>>
{
}
