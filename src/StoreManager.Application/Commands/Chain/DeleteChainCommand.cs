using StoreManager.Application.Data;
using StoreManager.Application.DTO.Chain.Command;
using StoreManager.Domain.Chain.ValueObjects;

namespace StoreManager.Application.Commands.Chain;

public class DeleteChainCommand : ICommand
{
    public DeleteChainCommand(ChainId id)
    {
        Id = id;
    }

    public DeleteChainCommand() { }

    public ChainId Id { get; set; }
}
