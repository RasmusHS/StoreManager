using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;

namespace StoreManager.Application.Commands.Chain;

public class CreateChainCommand : ICommand
{
    public CreateChainCommand(string name, List<CreateStoreCommand>? stores)
    {
        Name = name;
        Stores = stores;
    }

    public string Name { get; }

    public List<CreateStoreCommand>? Stores { get; }
}
