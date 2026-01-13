using EnsureThat;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Query;
using StoreManager.Domain.Chain.ValueObjects;

namespace StoreManager.Application.Queries.Chain;

public record GetChainQuery : IQuery<QueryChainDto>
{
    public GetChainQuery(ChainId id)
    {
        Ensure.That(id.Value, nameof(id.Value)).IsNotEmpty();

        Id = id;
    }

    public ChainId Id { get; private set; }
}
