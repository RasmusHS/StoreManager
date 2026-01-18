namespace StoreManager.Webapp.Client.Models.Store.Query;

public record QueryStoreDto
{
    public QueryStoreDto(Guid id, Guid? chainId, int number, string name, string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        ChainId = chainId;

        Number = number;
        Name = name;
        Street = street;
        PostalCode = postalCode;
        City = city;
        CountryCode = countryCode;
        PhoneNumber = phoneNumber;
        Email = email;
        FirstName = firstName;
        LastName = lastName;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public QueryStoreDto() { }

    public Guid Id { get; set; } // Store Identifier
    public Guid? ChainId { get; set; } // Associated Chain Identifier
    public int Number { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string PostalCode { get; set; }
    public string City { get; set; }
    public string CountryCode { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
