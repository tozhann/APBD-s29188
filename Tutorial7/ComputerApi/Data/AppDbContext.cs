using ComputerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ComputerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PC> PCs { get; set; }
    public DbSet<Component> Components { get; set; }
    public DbSet<ComponentType> ComponentTypes { get; set; }
    public DbSet<ComponentManufacturer> ComponentManufacturers { get; set; }
    public DbSet<PCComponent> PCComponents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── ComponentType ──────────────────────────────────────────────
        modelBuilder.Entity<ComponentType>(e =>
        {
            e.HasKey(ct => ct.Id);
            e.Property(ct => ct.Abbreviation).IsRequired().HasMaxLength(30);
            e.Property(ct => ct.Name).IsRequired().HasMaxLength(150);
        });

        // ── ComponentManufacturer ──────────────────────────────────────
        modelBuilder.Entity<ComponentManufacturer>(e =>
        {
            e.HasKey(cm => cm.Id);
            e.Property(cm => cm.Abbreviation).HasMaxLength(30);
            e.Property(cm => cm.FullName).HasMaxLength(300);
            e.Property(cm => cm.FoundationDate).HasColumnType("date");
        });

        // ── Component ──────────────────────────────────────────────────
        modelBuilder.Entity<Component>(e =>
        {
            e.HasKey(c => c.Code);
            e.Property(c => c.Code).HasColumnType("char(10)").IsRequired();
            e.Property(c => c.Name).IsRequired().HasMaxLength(300);
            e.Property(c => c.Description).HasColumnType("nvarchar(max)");

            e.HasOne(c => c.Manufacturer)
             .WithMany(m => m.Components)
             .HasForeignKey(c => c.ComponentManufacturersId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(c => c.ComponentType)
             .WithMany(ct => ct.Components)
             .HasForeignKey(c => c.ComponentTypesId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ── PC ─────────────────────────────────────────────────────────
        modelBuilder.Entity<PC>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).IsRequired().HasMaxLength(50);
            e.Property(p => p.Weight).HasColumnType("real");
            e.Property(p => p.CreatedAt).HasColumnType("datetime");
        });

        // ── PCComponent (compound PK, junction table) ──────────────────
        modelBuilder.Entity<PCComponent>(e =>
        {
            e.HasKey(pc => new { pc.PCId, pc.ComponentCode });

            e.HasOne(pc => pc.PC)
             .WithMany(p => p.PCComponents)
             .HasForeignKey(pc => pc.PCId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(pc => pc.Component)
             .WithMany(c => c.PCComponents)
             .HasForeignKey(pc => pc.ComponentCode)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Seed Data ──────────────────────────────────────────────────
        modelBuilder.Entity<ComponentType>().HasData(
            new ComponentType { Id = 1, Abbreviation = "CPU", Name = "Central Processing Unit" },
            new ComponentType { Id = 2, Abbreviation = "GPU", Name = "Graphics Processing Unit" },
            new ComponentType { Id = 3, Abbreviation = "RAM", Name = "Random Access Memory" }
        );

        modelBuilder.Entity<ComponentManufacturer>().HasData(
            new ComponentManufacturer { Id = 1, Abbreviation = "Intel", FullName = "Intel Corporation", FoundationDate = new DateOnly(1968, 7, 18) },
            new ComponentManufacturer { Id = 2, Abbreviation = "AMD", FullName = "Advanced Micro Devices", FoundationDate = new DateOnly(1969, 5, 1) },
            new ComponentManufacturer { Id = 3, Abbreviation = "NVIDIA", FullName = "NVIDIA Corporation", FoundationDate = new DateOnly(1993, 4, 5) }
        );

        modelBuilder.Entity<Component>().HasData(
            new Component { Code = "CPU001    ", Name = "Intel Core i9-13900K", Description = "High-end desktop processor", ComponentManufacturersId = 1, ComponentTypesId = 1 },
            new Component { Code = "GPU001    ", Name = "NVIDIA RTX 4090", Description = "Flagship gaming GPU", ComponentManufacturersId = 3, ComponentTypesId = 2 },
            new Component { Code = "RAM001    ", Name = "Corsair DDR5 32GB", Description = "High-speed DDR5 memory", ComponentManufacturersId = 2, ComponentTypesId = 3 }
        );

        modelBuilder.Entity<PC>().HasData(
            new PC { Id = 1, Name = "Gaming Beast X", Weight = 12.5f, Warranty = 36, CreatedAt = new DateTime(2026, 5, 8, 9, 0, 0), Stock = 5 },
            new PC { Id = 2, Name = "Office Mini Pro", Weight = 4.2f, Warranty = 24, CreatedAt = new DateTime(2026, 4, 15, 13, 30, 0), Stock = 12 },
            new PC { Id = 3, Name = "Workstation Ultra", Weight = 18.0f, Warranty = 48, CreatedAt = new DateTime(2026, 3, 1, 8, 0, 0), Stock = 3 }
        );

        modelBuilder.Entity<PCComponent>().HasData(
            new PCComponent { PCId = 1, ComponentCode = "CPU001    ", Amount = 1 },
            new PCComponent { PCId = 1, ComponentCode = "GPU001    ", Amount = 2 },
            new PCComponent { PCId = 2, ComponentCode = "RAM001    ", Amount = 2 },
            new PCComponent { PCId = 3, ComponentCode = "CPU001    ", Amount = 2 },
            new PCComponent { PCId = 3, ComponentCode = "GPU001    ", Amount = 4 }
        );
    }
}
