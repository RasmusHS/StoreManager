using FluentValidation;

namespace StoreManager.Application.DTO.Store.Command;

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

    public class Validator : AbstractValidator<UpdateStoreDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Store ID must not be empty.");
            RuleFor(x => x.ChainId).NotEmpty().WithMessage("Chain ID must not be empty.");
            RuleFor(x => x.Number).NotNull().WithMessage("Store number is required.");
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Store name is required.").MaximumLength(100).WithMessage("Store name must not exceed 100 characters.");
            RuleFor(x => x.Street).NotEmpty().NotNull().WithMessage("Street is required.").MaximumLength(200).WithMessage("Street must not exceed 200 characters.");
            RuleFor(x => x.PostalCode).NotEmpty().NotNull().WithMessage("Postal code is required.").MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.");
            RuleFor(x => x.City).NotEmpty().NotNull().WithMessage("City is required.").MaximumLength(100).WithMessage("City must not exceed 100 characters.");
            RuleFor(x => x.CountryCode).NotEmpty().NotNull().WithMessage("Country code is required.").MaximumLength(10).WithMessage("Country code must not exceed 10 characters.");
            RuleFor(x => x.PhoneNumber).NotEmpty().NotNull().WithMessage("Phone number is required.").MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.");
            RuleFor(x => x.Email).NotEmpty().NotNull().WithMessage("Email is required.").EmailAddress().WithMessage("Email must be a valid email address.").MaximumLength(100).WithMessage("Email must not exceed 100 characters.");
            RuleFor(x => x.FirstName).NotEmpty().NotNull().WithMessage("First name is required.").MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
            RuleFor(x => x.LastName).NotEmpty().NotNull().WithMessage("Last name is required.").MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
        }
    }
}
