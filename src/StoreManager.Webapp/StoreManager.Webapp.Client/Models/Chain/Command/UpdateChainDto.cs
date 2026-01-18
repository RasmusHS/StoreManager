namespace StoreManager.Webapp.Client.Models.Chain.Command;

public record UpdateChainDto
{
    public UpdateChainDto(Guid id, string name, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        Name = name;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public UpdateChainDto() { }

    public Guid Id { get; set; } // Chain unique identifier
    public string Name { get; set; } // Unique name of the chain
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
