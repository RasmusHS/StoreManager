using EnsureThat;

namespace StoreManager.Domain.Common.ValueObjects;

public class Address : ValueObject
{
    public string Street { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }

    private Address(string street, string postalcode, string city)
    {
        Street = street;
        PostalCode = postalcode;
        City = city;
    }

    public Address() { } //for ORM

    public static Result<Address> Create(string street, string postalcode, string city)
    {
        Ensure.That(street, nameof(street)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(postalcode, nameof(postalcode)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();
        Ensure.That(city, nameof(city)).IsNotNullOrEmpty().IsNotNullOrWhiteSpace();

        return Result.Ok<Address>(new Address(street, postalcode, city));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return PostalCode;
        yield return City;
    }
}
