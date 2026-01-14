using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;

namespace StoreManager.Persistence.Configs;

public class ChainConfig : IEntityTypeConfiguration<ChainEntity>
{
    public void Configure(EntityTypeBuilder<ChainEntity> builder)
    {
        // Configure keys
        builder.HasKey(c => c.Id); // Primary Key
        builder.Property(c => c.Id).HasConversion(
            chain => chain.Value,
            value => ChainId.GetExisting(value).Value!); // Value Object Conversion

        builder.HasIndex(c => c.Name).IsUnique(true); // Unique Index on Name
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100); 

        builder.HasIndex(c => c.CreatedOn).IsUnique(false);

        // Configure relationships
        builder.HasMany(c => c.Stores)
            .WithOne(s => s.Chain)
            .HasForeignKey(s => s.ChainId);

        // Configure concurrency token
        builder.Property(c => c.ModifiedOn).IsConcurrencyToken(true);
    }
}
