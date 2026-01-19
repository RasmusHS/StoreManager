using System.ComponentModel.DataAnnotations;

namespace StoreManager.Webapp.Client.Models.Store.Command;

public record UpdateStoreDto
{
    public UpdateStoreDto(Guid id, Guid? chainId, int number, string name, string street, string postalCode, string city, string countryCode, string phoneNumber, string email, string firstName, string lastName, DateTime createdOn, DateTime modifiedOn)
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

    public UpdateStoreDto() { }

    public Guid Id { get; set; } // Store Identifier
    public Guid? ChainId { get; set; } // Associated Chain Identifier

    [Required]
    public int Number { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    [Required]
    public string Street { get; set; }

    [Required]
    public string PostalCode { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string CountryCode { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
