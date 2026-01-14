using EnsureThat;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;
using System.Net;

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
        //Ensure.That(, nameof());
        if (chainId != null)
        {
            Ensure.That(chainId.Value, nameof(chainId.Value)).IsNotEmpty();
        }  
        Ensure.That(number, nameof(number));
        Ensure.That(name, nameof(name)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(address.Street, nameof(address.Street)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(address.PostalCode, nameof(address.PostalCode)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(address.City, nameof(address.City)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(phoneNumber.CountryCode, nameof(phoneNumber.CountryCode)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(phoneNumber.Number, nameof(phoneNumber.Number)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(email.Value, nameof(email.Value)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(storeOwner.FirstName, nameof(storeOwner.FirstName)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(storeOwner.LastName, nameof(storeOwner.LastName)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();

        return Result.Ok<StoreEntity>(new StoreEntity(StoreId.Create().Value, chainId, number, name, address, phoneNumber, email, storeOwner));
    }

    public void UpdateStore(ChainId? chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner /*string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName*/)
    {
        Ensure.That(number, nameof(number));
        Ensure.That(name, nameof(name)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(address.Street, nameof(address.Street)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(address.PostalCode, nameof(address.PostalCode)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(address.City, nameof(address.City)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(phoneNumber.CountryCode, nameof(phoneNumber.CountryCode)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(phoneNumber.Number, nameof(phoneNumber.Number)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(email.Value, nameof(email.Value)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(storeOwner.FirstName, nameof(storeOwner.FirstName)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(storeOwner.LastName, nameof(storeOwner.LastName)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        //Ensure.That(street, nameof(street)).IsNotNullOrEmpty();
        //Ensure.That(postalCode, nameof(postalCode)).IsNotNullOrEmpty();
        //Ensure.That(city, nameof(city)).IsNotNullOrEmpty();
        //Ensure.That(countryCode, nameof(countryCode)).IsNotNullOrEmpty();
        //Ensure.That(phoneNumber, nameof(phoneNumber)).IsNotNullOrEmpty();
        //Ensure.That(email, nameof(email)).IsNotNullOrEmpty();
        //Ensure.That(firstName, nameof(firstName));
        //Ensure.That(lastName, nameof(lastName));

        ChainId = chainId;
        Number = number;
        Name = name;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        StoreOwner = storeOwner;
        //Address = Address.Create(street, postalCode, city).Value;
        //PhoneNumber = PhoneNumber.Create(countryCode, phoneNumber).Value;
        //Email = Email.Create(email).Value;
        //StoreOwner = FullName.Create(firstName, lastName).Value;

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
