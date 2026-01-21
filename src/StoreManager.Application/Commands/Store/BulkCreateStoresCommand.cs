using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;

namespace StoreManager.Application.Commands.Store;

public class BulkCreateStoresCommand : ICommand<CollectionResponseBase<StoreResponseDto>>
{
    public BulkCreateStoresCommand(List<CreateStoreCommand> stores)
    {
        Stores = stores;
    }

    public BulkCreateStoresCommand() { }

    public List<CreateStoreCommand> Stores { get; set; }
}
