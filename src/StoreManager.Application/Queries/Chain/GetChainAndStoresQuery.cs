using EnsureThat;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Query;
using StoreManager.Domain.Chain.ValueObjects;

namespace StoreManager.Application.Queries.Chain;

public record GetChainAndStoresQuery : IQuery<QueryChainDto>
{
    public GetChainAndStoresQuery(ChainId id)
    {
        Ensure.That(id.Value, nameof(id.Value)).IsNotEmpty();

        Id = id;
    }

    public ChainId Id { get; private set; }
}
