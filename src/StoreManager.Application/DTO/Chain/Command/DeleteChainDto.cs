using FluentValidation;

namespace StoreManager.Application.DTO.Chain.Command;

public record DeleteChainDto
{
    public DeleteChainDto(Guid id, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public DeleteChainDto() { }

    public Guid Id { get; set; } // Chain unique identifier
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public class Validator : AbstractValidator<DeleteChainDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Chain ID must not be empty.");
        }
    }
}
