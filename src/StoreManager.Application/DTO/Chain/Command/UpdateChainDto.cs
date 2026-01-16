using FluentValidation;

namespace StoreManager.Application.DTO.Chain.Command;

public record UpdateChainDto
{
    public UpdateChainDto(Guid id, string name, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        Name = name;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public UpdateChainDto() { }

    public Guid Id { get; set; } // Chain unique identifier
    public string Name { get; set; } // Unique name of the chain
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public class Validator : AbstractValidator<UpdateChainDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Chain ID must not be empty.");
            RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Chain name must not be empty.").MaximumLength(100).WithMessage("Chain name must not exceed 100 characters.");
        }
    }
}
