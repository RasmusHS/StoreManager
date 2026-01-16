using StoreManager.Application.DTO.Store.Command;

namespace StoreManager.Application.DTO.Chain.Command;

public record ChainResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<StoreResponseDto>? Stores { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}
