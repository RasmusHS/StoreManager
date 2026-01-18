using StoreManager.Webapp.Client.Models.Store.Command;

namespace StoreManager.Webapp.Client.Models.Chain.Command;

public record ChainResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StoreResponseDto>? Stores { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
