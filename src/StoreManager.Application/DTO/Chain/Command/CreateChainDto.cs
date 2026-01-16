using FluentValidation;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.DTO.Chain.Command;

public record CreateChainDto
{
    public CreateChainDto(string name, List<CreateStoreDto>? stores)
    {
        Name = name;

        Stores = stores;
    }

    public CreateChainDto(string name)
    {
        Name = name;
    }

    public CreateChainDto() { }

    public string Name { get; set; }
    public List<CreateStoreDto>? Stores { get; set; }

    public class Validator : AbstractValidator<CreateChainDto>
    {
        public Validator(bool containsStores)
        {
            if (containsStores == true)
            {
                RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Chain name is required.").MaximumLength(100).WithMessage("Chain name must not exceed 100 characters.");
                RuleForEach(x => x.Stores).SetValidator(new CreateStoreDto.Validator());
            }
            else
            {
                RuleFor(x => x.Name).NotEmpty().NotNull().WithMessage("Chain name is required.").MaximumLength(100).WithMessage("Chain name must not exceed 100 characters.");
            }
        }
    }
}
