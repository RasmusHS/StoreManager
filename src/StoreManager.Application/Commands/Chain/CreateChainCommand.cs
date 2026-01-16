using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Command;

namespace StoreManager.Application.Commands.Chain;

public class CreateChainCommand : ICommand<ChainResponseDto>
{
    public CreateChainCommand(string name, List<CreateStoreCommand>? stores)
    {
        Name = name;
        Stores = stores;
    }

    public string Name { get; }

    public List<CreateStoreCommand>? Stores { get; }
}
