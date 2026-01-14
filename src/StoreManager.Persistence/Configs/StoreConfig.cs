using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Persistence.Configs;

public class StoreConfig : IEntityTypeConfiguration<StoreEntity>
{
    public void Configure(EntityTypeBuilder<StoreEntity> builder)
    {
        // Configure keys
        builder.HasKey(p => p.Id); // Primary Key
        builder.Property(p => p.Id).HasConversion(
            store => store.Value,
            value => StoreId.GetExisting(value).Value!); // Value Object Conversion

        builder.HasIndex(s => s.Number).IsUnique(true);
        builder.HasIndex(s => s.Name).IsUnique(false);

        // Configure strongly typed properties
        builder.OwnsOne(s => s.Address, propertyBuilder =>
        {
            propertyBuilder.Property(p => p.Street).HasMaxLength(50);
            propertyBuilder.Property(p => p.PostalCode).HasMaxLength(10);
            propertyBuilder.Property(p => p.City).HasMaxLength(100);
        });

        builder.OwnsOne(s => s.PhoneNumber, propertyBuilder =>
        {
            propertyBuilder.Property(p => p.CountryCode).HasMaxLength(8);
            propertyBuilder.Property(p => p.Number).HasMaxLength(12);
        });

        builder.Property(s => s.Email).HasConversion(
            email => email.Value,
            value => Email.Create(value, true).Value!).HasMaxLength(100);

        builder.OwnsOne(s => s.StoreOwner, propertyBuilder =>
        {
            propertyBuilder.Property(p => p.FirstName).HasMaxLength(20);
            propertyBuilder.Property(p => p.LastName).HasMaxLength(20);
        });

        // Configure relationships
        builder.HasOne(s => s.Chain)
            .WithMany(c => c.Stores)
            .HasForeignKey(s => s.ChainId);

        // Configure concurrency token
        builder.Property(s => s.ModifiedOn).IsConcurrencyToken(true);
    }
}
