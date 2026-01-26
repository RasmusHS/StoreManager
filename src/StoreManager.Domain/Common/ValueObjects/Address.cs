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
        List<Error> errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(street))
            errors.Add(Errors.General.ValueIsRequired(nameof(street)));
        if (string.IsNullOrWhiteSpace(postalcode))
            errors.Add(Errors.General.ValueIsRequired(nameof(postalcode)));
        if (string.IsNullOrWhiteSpace(city))
            errors.Add(Errors.General.ValueIsRequired(nameof(city)));

        if (errors.Any())
            return Result.Fail<Address>(errors);
        else
            return Result.Ok<Address>(new Address(street, postalcode, city));
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return PostalCode;
        yield return City;
    }
}
