using FluentValidation;

namespace StoreManager.Application.DTO.Store.Command;

public record DeleteStoreDto
{
    public DeleteStoreDto(Guid id, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public DeleteStoreDto() { }

    public Guid Id { get; set; } // Store Identifier
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public class Validator : AbstractValidator<DeleteStoreDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Store ID must not be empty.");
        }
    }
}
