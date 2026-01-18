using StoreManager.Webapp.Client.Models.Store.Query;

namespace StoreManager.Webapp.Client.Models.Chain.Query;

public record QueryChainDto
{
    public QueryChainDto(Guid id, string name, List<QueryStoreDto>? stores, int? storeCount, DateTime createdOn, DateTime modifiedOn)
    {
        Id = id;
        Name = name;
        Stores = stores;
        StoreCount = storeCount;

        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public QueryChainDto() { }

    public Guid Id { get; set; } // Chain unique identifier
    public string Name { get; set; } // Unique name of the chain
    public List<QueryStoreDto>? Stores { get; set; } = new(); // List of stores associated with the chain
    public int? StoreCount { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}

