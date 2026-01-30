using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;

namespace StoreManager.Domain.Chain;

public sealed class ChainEntity : AggregateRoot<ChainId>
{
    // Constructors
    internal ChainEntity() { } // For ORM

    private ChainEntity(ChainId id, string name) : base(id)
    {
        Id = id;
        Name = name;

        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
    }

    public static Result<ChainEntity> Create(string name)
    {
        List<Error> errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsRequired(nameof(name)));

        if (errors.Count > 0)
            return Result.Fail<ChainEntity>(errors);
        else
            return Result.Ok<ChainEntity>(new ChainEntity(ChainId.Create().Value, name));
    }

    public void AddRangeStoresToChain(List<StoreEntity> stores)
    {
        List<Error> errors = new List<Error>();

        if (stores == null)
            errors.Add(Errors.General.ValueIsRequired(nameof(stores)));

        if (errors.Count > 0) // Throw ArgumentException for now, but should return Result type 
            throw new ArgumentException(string.Join("; ", errors.Select(e => e.Code), errors.Select(e => e.Message), errors.Select(e => e.StatusCode)));

        _stores.AddRange(stores);
    }

    public void UpdateChainDetails(string name)
    {
        List<Error> errors = new List<Error>();
        
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsRequired(nameof(name)));

        if (errors.Count > 0)
            throw new ArgumentException(string.Join("; ", errors.Select(e => e.Code), errors.Select(e => e.Message), errors.Select(e => e.StatusCode)));

        Name = name;

        ModifiedOn = DateTime.UtcNow;
    }

    // Properties
    public string Name { get; private set; } // Unique name of the chain


    // Aggregate members - Navigational properties 
    private readonly List<StoreEntity> _stores = new();
    public IReadOnlyList<StoreEntity> Stores => _stores.AsReadOnly(); // one-to-many
}
