using System.Text.Json.Serialization;

namespace StoreManager.Webapp.Client.Models;

public class CollectionResponseBase<T>
{
    [JsonPropertyName("data")]
    public IEnumerable<T> Data { get; set; } = new List<T>();

    // Parameterless constructor for JSON deserialization
    public CollectionResponseBase()
    {
    }
}
