using StoreManager.Application.Data;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Commands.Store;

public class DeleteStoreCommand : ICommand
{
    public DeleteStoreCommand(StoreId id)
    {
        Id = id;
    }

    public DeleteStoreCommand(ChainId chainId)
    {
        ChainId = chainId;
    }

    public DeleteStoreCommand() { }

    public StoreId Id { get; set; }
    public ChainId ChainId { get; set; }
}
