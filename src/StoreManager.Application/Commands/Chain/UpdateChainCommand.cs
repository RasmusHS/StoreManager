using StoreManager.Application.Commands.Store;
using StoreManager.Application.Data;
using StoreManager.Domain.Chain.ValueObjects;

namespace StoreManager.Application.Commands.Chain;

public class UpdateChainCommand : ICommand
{
    public UpdateChainCommand(ChainId id, string name, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        Name = name;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public UpdateChainCommand() { }

    public ChainId Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
