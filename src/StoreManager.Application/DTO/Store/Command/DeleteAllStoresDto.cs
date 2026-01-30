using FluentValidation;

namespace StoreManager.Application.DTO.Store.Command;

public record DeleteAllStoresDto
{
    public DeleteAllStoresDto(Guid chainId, DateTime createdOn, DateTime modifiedOn)
    {
        ChainId = chainId;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public Guid ChainId { get; set; } // Associated Chain Identifier
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public class Validator : AbstractValidator<DeleteAllStoresDto>
    {
        public Validator()
        {
            RuleFor(x => x.ChainId).NotEmpty().WithMessage("Chain ID must not be empty.");
        }
    }
}
