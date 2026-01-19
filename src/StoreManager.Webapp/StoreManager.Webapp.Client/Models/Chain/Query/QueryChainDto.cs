using StoreManager.Webapp.Client.Models.Store.Query;
using System.Text.Json.Serialization;

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

    [JsonPropertyName("id")]
    public Guid Id { get; set; } // Chain unique identifier

    [JsonPropertyName("name")]
    public string Name { get; set; } // Unique name of the chain

    [JsonPropertyName("stores")]
    public List<QueryStoreDto>? Stores { get; set; } = new(); // List of stores associated with the chain

    [JsonPropertyName("storeCount")]
    public int? StoreCount { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; }

    [JsonPropertyName("modifiedOn")]
    public DateTime ModifiedOn { get; set; }
}

