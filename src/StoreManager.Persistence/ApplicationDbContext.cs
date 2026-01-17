using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Data;
using StoreManager.Domain.Chain;
using StoreManager.Domain.Chain.ValueObjects;
using StoreManager.Domain.Common.ValueObjects;
using StoreManager.Domain.Store;
using StoreManager.Domain.Store.ValueObjects;

namespace StoreManager.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<ChainEntity> ChainEntities { get; set; }
    public DbSet<StoreEntity> StoreEntities { get; set; }

    public void SaveChanges(CancellationToken cancellationToken = default)
    {
        base.SaveChanges();
    }

    public override int SaveChanges()
    {
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }

    Task IApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        // Suppress pending model changes warning in Development
        if (environment == "Development")
        {
            optionsBuilder.ConfigureWarnings(warnings =>
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Only apply seed data if not in testing environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Testing")
        {
            // Seed data for Chains
            var chainOptiView = ChainId.GetExisting(Guid.Parse("5ec199f0-4c26-4d20-bbdd-39ac844237b8")).Value;
            modelBuilder.Entity<ChainEntity>().HasData(new
            {
                Id = chainOptiView,
                Name = "OptiView",
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });

            var chainFocalPoint = ChainId.GetExisting(Guid.Parse("d3271c28-5e1d-4011-9f79-aa573a9f2e3c")).Value;
            modelBuilder.Entity<ChainEntity>().HasData(new
            {
                Id = chainFocalPoint,
                Name = "FocalPoint",
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });

            var chainFocusOptics = ChainId.GetExisting(Guid.Parse("e887f9ce-2096-4832-8587-f96d9c7d8bc7")).Value;
            modelBuilder.Entity<ChainEntity>().HasData(new
            {
                Id = chainFocusOptics,
                Name = "FocusOptics",
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });

            var chainClearSight = ChainId.GetExisting(Guid.Parse("7eaf61d2-6d20-4ef7-9e2c-3e230456ae3c")).Value;
            modelBuilder.Entity<ChainEntity>().HasData(new
            {
                Id = chainClearSight,
                Name = "ClearSight",
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });

            // Seed data for Stores
            var chainOptiViewStore1 = StoreId.GetExisting(Guid.Parse("82d72abf-0045-4aaa-9d8b-78f28b437052")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainOptiViewStore1,
                ChainId = chainOptiView,
                Number = 1,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainOptiViewStore1,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainOptiViewStore1,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainOptiViewStore1,
                    FirstName = "John",
                    LastName = "Doe"
                });

            var chainOptiViewStore2 = StoreId.GetExisting(Guid.Parse("6c458b7e-c560-47c0-9e7c-5e66adf87192")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainOptiViewStore2,
                ChainId = chainOptiView,
                Number = 2,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainOptiViewStore2,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainOptiViewStore2,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainOptiViewStore2,
                    FirstName = "John",
                    LastName = "Doe"
                });

            var chainFocalPointStore1 = StoreId.GetExisting(Guid.Parse("d84a8b08-7d18-4c23-a8ac-efc11c585d74")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainFocalPointStore1,
                ChainId = chainFocalPoint,
                Number = 3,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainFocalPointStore1,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainFocalPointStore1,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainFocalPointStore1,
                    FirstName = "John",
                    LastName = "Doe"
                });
            var chainFocalPointStore2 = StoreId.GetExisting(Guid.Parse("7d7e8194-b981-478a-baee-57982fa3f699")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainFocalPointStore2,
                ChainId = chainFocalPoint,
                Number = 4,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainFocalPointStore2,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainFocalPointStore2,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainFocalPointStore2,
                    FirstName = "John",
                    LastName = "Doe"
                });

            var chainFocusOpticsStore1 = StoreId.GetExisting(Guid.Parse("2ecacee5-f3c8-43e0-9ca6-379c49248afb")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainFocusOpticsStore1,
                ChainId = chainFocusOptics,
                Number = 5,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainFocusOpticsStore1,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainFocusOpticsStore1,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainFocusOpticsStore1,
                    FirstName = "John",
                    LastName = "Doe"
                });
            var chainFocusOpticsStore2 = StoreId.GetExisting(Guid.Parse("2277a2ce-6623-4879-a3b5-709ba9fa625e")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainFocusOpticsStore2,
                ChainId = chainFocusOptics,
                Number = 6,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainFocusOpticsStore2,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainFocusOpticsStore2,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainFocusOpticsStore2,
                    FirstName = "John",
                    LastName = "Doe"
                });

            var chainClearSightStore1 = StoreId.GetExisting(Guid.Parse("4a15bcc2-9104-46e7-8a6c-b2030c1d573b")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainClearSightStore1,
                ChainId = chainClearSight,
                Number = 7,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainClearSightStore1,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainClearSightStore1,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainClearSightStore1,
                    FirstName = "John",
                    LastName = "Doe"
                });
            var chainClearSightStore2 = StoreId.GetExisting(Guid.Parse("23273be7-91ad-4095-a891-f10f0633a420")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = chainClearSightStore2,
                ChainId = chainClearSight,
                Number = 8,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = chainClearSightStore2,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = chainClearSightStore2,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = chainClearSightStore2,
                    FirstName = "John",
                    LastName = "Doe"
                });

            var NoChainStore1 = StoreId.GetExisting(Guid.Parse("1e8d0a09-b7b9-4a94-a293-cff36e767bea")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = NoChainStore1,
                //ChainId = ,
                Number = 9,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = NoChainStore1,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = NoChainStore1,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = NoChainStore1,
                    FirstName = "John",
                    LastName = "Doe"
                });
            var NoChainStore2 = StoreId.GetExisting(Guid.Parse("a2f309d9-e908-4ee1-9365-b72f7d088764")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = NoChainStore2,
                //ChainId = ,
                Number = 10,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = NoChainStore2,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = NoChainStore2,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = NoChainStore2,
                    FirstName = "John",
                    LastName = "Doe"
                });
            var NoChainStore3 = StoreId.GetExisting(Guid.Parse("e0aa46d0-3e3f-43e3-beb0-d71dfbc8500a")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = NoChainStore3,
                //ChainId = ,
                Number = 11,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = NoChainStore3,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = NoChainStore3,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = NoChainStore3,
                    FirstName = "John",
                    LastName = "Doe"
                });
            var NoChainStore4 = StoreId.GetExisting(Guid.Parse("ea911dd7-044d-45ff-b380-e04b5a8c1aa9")).Value;
            modelBuilder.Entity<StoreEntity>().HasData(new
            {
                Id = NoChainStore4,
                //ChainId = ,
                Number = 12,
                Name = "OptiView Downtown",
                Email = Email.Create("TestMail@OptiView.dk").Value,
                CreatedOn = DateTime.Parse("1/12/2026 4:22:44 PM"),
                ModifiedOn = DateTime.Parse("1/12/2026 4:22:44 PM")
            });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.Address)
                .HasData(new
                {
                    StoreEntityId = NoChainStore4,
                    Street = "TestStreet 1",
                    PostalCode = "9000",
                    City = "Aalborg"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.PhoneNumber)
                .HasData(new
                {
                    StoreEntityId = NoChainStore4,
                    CountryCode = "+45",
                    Number = "12345678"
                });
            modelBuilder.Entity<StoreEntity>()
                .OwnsOne(s => s.StoreOwner)
                .HasData(new
                {
                    StoreEntityId = NoChainStore4,
                    FirstName = "John",
                    LastName = "Doe"
                });
        }
    }

    private byte[] ImageToByteArray(string imagePath)
    {
        return File.ReadAllBytes(imagePath);
    }
}
