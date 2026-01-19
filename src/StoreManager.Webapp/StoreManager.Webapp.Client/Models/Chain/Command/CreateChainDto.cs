using StoreManager.Webapp.Client.Models.Store.Command;
using System.ComponentModel.DataAnnotations;

namespace StoreManager.Webapp.Client.Models.Chain.Command;

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

    [Required(ErrorMessage = "Chain name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
    public string Name { get; set; }
    public List<CreateStoreDto>? Stores { get; set; }
}
