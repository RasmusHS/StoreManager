using StoreManager.Webapp.Client.Models.Store.Command;

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

    public string Name { get; set; }
    public List<CreateStoreDto>? Stores { get; set; }
}
