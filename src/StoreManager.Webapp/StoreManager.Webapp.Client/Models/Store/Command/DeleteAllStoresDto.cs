namespace StoreManager.Webapp.Client.Models.Store.Command;

public record DeleteAllStoresDto
{
    public DeleteAllStoresDto(Guid chainId, DateTime createdOn, DateTime modifiedOn)
    {
        ChainId = chainId;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public Guid ChainId { get; set; } // Associated Chain Identifier
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
