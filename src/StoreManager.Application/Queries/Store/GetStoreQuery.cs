using EnsureThat;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Query;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Queries.Store;

public record GetStoreQuery : IQuery<QueryStoreDto>
{
    public GetStoreQuery(StoreId id)
    {
        Ensure.That(id.Value, nameof(id.Value)).IsNotEmpty();

        Id = id;
    }

    public StoreId Id { get; private set; }
}
