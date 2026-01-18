namespace StoreManager.Webapp.Client.Models.Chain.Command;

public record DeleteChainDto
{
    public DeleteChainDto(Guid id, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public DeleteChainDto() { }

    public Guid Id { get; set; } // Chain unique identifier
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
