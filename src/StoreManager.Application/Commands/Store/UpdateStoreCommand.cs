using StoreManager.Application.Data;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Commands.Store;

public class UpdateStoreCommand : ICommand
{
    public UpdateStoreCommand(StoreId id, ChainId chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner, /*string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName,*/ DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        ChainId = chainId;
        Number = number;
        Name = name;
        
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        StoreOwner = storeOwner;
        //Address = Address.Create(address.Street, address.PostalCode, address.City);
        //PhoneNumber = PhoneNumber.Create(phoneNumber.CountryCode, phoneNumber.Number);
        //Email = Email.Create(email.Value);
        //StoreOwner = FullName.Create(storeOwner.FirstName, storeOwner.LastName);

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public UpdateStoreCommand() { }

    public StoreId Id { get; set; }
    public ChainId ChainId { get; set; }
    public int Number { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public PhoneNumber PhoneNumber { get; set; }
    public Email Email { get; set; }
    public FullName StoreOwner { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
    //public string Street { get; set; }
    //public string PostalCode { get; set; }
    //public string City { get; set; }
    //public string CountryCode { get; set; }
    //public string PhoneNumber { get; set; }
    //public string Email { get; set; }
    //public string FirstName { get; set; }
    //public string LastName { get; set; }
}
