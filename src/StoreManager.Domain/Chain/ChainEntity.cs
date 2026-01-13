using EnsureThat;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;
using StoreManager.Domain.Store;

namespace StoreManager.Domain.Chain;

public sealed class ChainEntity : AggregateRoot<ChainId>
{
    // Constructors
    internal ChainEntity() {}

    private ChainEntity(ChainId id, string name) : base(id)
    {
        Id = id;
        Name = name;

        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
    }

    public static Result<ChainEntity> Create(string name)
    {
        //Ensure.That(, nameof());
        Ensure.That(name, nameof(name)).IsNotNullOrEmpty;

        return Result.Ok<ChainEntity>(new ChainEntity(ChainId.Create().Value, name));
    }

    public void AddStoreToChain(StoreEntity store)
    {
        Ensure.That(store, nameof(store)).IsNotNull();
        _stores.Add(store);
    }

    public void AddRangeStoresToChain(List<StoreEntity> stores)
    {
        Ensure.That(stores, nameof(stores)).IsNotNull();
        _stores.AddRange(stores);
    }

    public void UpdateChainDetails(string name)
    {
        Ensure.That(name, nameof(name)).IsNotNullOrEmpty;

        ModifiedOn = DateTime.UtcNow;
    }

    // Properties
    public string Name { get; private set; } // Unique name of the chain


    // Aggregate members - Navigational properties 
    private readonly List<StoreEntity> _stores = new();
    public IReadOnlyList<StoreEntity> Stores => _stores.AsReadOnly(); // one-to-many
}
