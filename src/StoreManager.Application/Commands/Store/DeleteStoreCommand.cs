using StoreManager.Application.Data;
using StoreManager.Application.DTO.Store.Command;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Application.Commands.Store;

public class DeleteStoreCommand : ICommand
{
    public DeleteStoreCommand(StoreId id)
    {
        Id = id;
    }

    public StoreId Id { get; set; }
}
