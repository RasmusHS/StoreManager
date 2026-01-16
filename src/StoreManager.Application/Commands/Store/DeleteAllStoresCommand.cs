using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Chain.ValueObjects;

namespace StoreManager.Application.Commands.Store;

public class DeleteAllStoresCommand : ICommand
{
    public DeleteAllStoresCommand(ChainId chainId)
    {
        ChainId = chainId;
    }

    public ChainId ChainId { get; set; }
}
