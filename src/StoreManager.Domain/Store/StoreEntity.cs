using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Domain.Store;

public sealed class StoreEntity : Entity<StoreId>
{
    // Constructors
    internal StoreEntity() { } // For ORM

    private StoreEntity(StoreId id, ChainId? chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner) : base(id)
    {
        Id = id;
        ChainId = chainId;
        Number = number;
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        StoreOwner = storeOwner;

        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
    }

    public static Result<StoreEntity> Create(ChainId? chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner)
    {
        List<Error> errors = new List<Error>();
        if (chainId != null)
        {
            if (chainId.Value == Guid.Empty)
                errors.Add(Errors.General.ValueIsRequired(nameof(chainId)));
        }  
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsRequired(nameof(name)));
        // Value objects will/should validate themselves

        if (errors.Any())
            return Result.Fail<StoreEntity>(errors);
        else
            return Result.Ok<StoreEntity>(new StoreEntity(StoreId.Create().Value, chainId, number, name, address, phoneNumber, email, storeOwner));
    }

    public void UpdateStore(ChainId? chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner)
    {
        List<Error> errors = new List<Error>();
        if (chainId?.Value == Guid.Empty)
            errors.Add(Errors.General.ValueIsRequired(nameof(chainId)));
        if (string.IsNullOrWhiteSpace(name))
            errors.Add(Errors.General.ValueIsRequired(nameof(name)));
        // Value objects will/should validate themselves

        if (errors.Any()) // Throw ArgumentException for now, but should return Result type 
            throw new ArgumentException(string.Join("; ", errors.Select(e => e.Code), errors.Select(e => e.Message), errors.Select(e => e.StatusCode)));

        ChainId = chainId;
        Number = number;
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        StoreOwner = storeOwner;

        ModifiedOn = DateTime.UtcNow;
    }

    // Properties
    public ChainId? ChainId { get; private set; }
    public int Number { get; private set; } // Unique store number within the chain
    public string Name { get; private set; }
    public Address Address { get; private set; }
    public PhoneNumber PhoneNumber { get; private set; }
    public Email Email { get; private set; }
    public FullName StoreOwner { get; private set; }


    // Navigation properties
    public ChainEntity Chain { get; private set; }
}
