namespace StoreManager.Webapp.Client.Models.Store.Command;

public record DeleteStoreDto
{
    public DeleteStoreDto(Guid id, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public DeleteStoreDto() { }

    public Guid Id { get; set; } // Store Identifier
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
