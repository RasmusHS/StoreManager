using StoreManager.Application.Data;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;

namespace StoreManager.Application.Commands.Store;

public class CreateStoreCommand : ICommand
{
    public CreateStoreCommand(ChainId? chainId, int number, string name, Address address, PhoneNumber phoneNumber, Email email, FullName storeOwner/*string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName*/)
    {
        ChainId = chainId;
        Number = number;
        Name = name;

        //Street = street;
        //PostalCode = postalCode;
        //City = city;
        //CountryCode = countryCode;
        //PhoneNumber = phoneNumber;
        //Email = email;
        //FirstName = firstName;
        //LastName = lastName;
        Address = address;
        PhoneNumber = phoneNumber;
        Email = email;
        StoreOwner = storeOwner;
        //Address = Address.Create(address.Street, address.PostalCode, address.City);
        //PhoneNumber = PhoneNumber.Create(phoneNumber.CountryCode, phoneNumber.Number);
        //Email = Email.Create(email.Value);
        //StoreOwner = FullName.Create(storeOwner.FirstName, storeOwner.LastName);
    }

    public CreateStoreCommand() { }

    public ChainId? ChainId { get; set; }
    public int Number { get; }
    public string Name { get; }
    public Address Address { get; }
    public PhoneNumber PhoneNumber { get; }
    public Email Email { get; }
    public FullName StoreOwner { get; }
    //public string Street { get; }
    //public string PostalCode { get; }
    //public string City { get; }
    //public string CountryCode { get; }
    //public string PhoneNumber { get; }
    //public string Email { get; }
    //public string FirstName { get; }
    //public string LastName { get; }
}
