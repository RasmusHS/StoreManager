using EnsureThat;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Chain.ValueObjects;

namespace StoreManager.Application.Queries.Store;

public record GetAllStoresByChainQuery : IQuery<CollectionResponseBase<QueryStoreDto>>
{
    public GetAllStoresByChainQuery(ChainId chainId)
    {
        Ensure.That(chainId.Value, nameof(chainId.Value)).IsNotEmpty();
        ChainId = chainId;
    }

    public GetAllStoresByChainQuery() { }

    public ChainId ChainId { get; }
}
