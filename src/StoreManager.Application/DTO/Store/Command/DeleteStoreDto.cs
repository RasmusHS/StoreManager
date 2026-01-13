using FluentValidation;

namespace StoreManager.Application.DTO.Store.Command;

public record DeleteStoreDto
{
    public DeleteStoreDto(Guid id, Guid chainId, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        ChainId = chainId;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public DeleteStoreDto() { }

    public Guid Id { get; set; } // Store Identifier
    public Guid ChainId { get; set; } // Associated Chain Identifier
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }

    public class Validator : AbstractValidator<DeleteStoreDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Store ID must not be empty.");
            RuleFor(x => x.ChainId).NotEmpty().WithMessage("Chain ID must not be empty.");
        }
    }
}
