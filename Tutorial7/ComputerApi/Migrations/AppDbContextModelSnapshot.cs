using System;
using ComputerApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ComputerApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ComputerApi.Models.ComponentManufacturer", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                b.Property<string>("Abbreviation").IsRequired().HasMaxLength(30).HasColumnType("nvarchar(30)");
                b.Property<DateOnly>("FoundationDate").HasColumnType("date");
                b.Property<string>("FullName").IsRequired().HasMaxLength(300).HasColumnType("nvarchar(300)");
                b.HasKey("Id");
                b.ToTable("ComponentManufacturers");
                b.HasData(
                    new { Id = 1, Abbreviation = "Intel", FullName = "Intel Corporation", FoundationDate = new DateOnly(1968, 7, 18) },
                    new { Id = 2, Abbreviation = "AMD", FullName = "Advanced Micro Devices", FoundationDate = new DateOnly(1969, 5, 1) },
                    new { Id = 3, Abbreviation = "NVIDIA", FullName = "NVIDIA Corporation", FoundationDate = new DateOnly(1993, 4, 5) });
            });

            modelBuilder.Entity("ComputerApi.Models.ComponentType", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                b.Property<string>("Abbreviation").IsRequired().HasMaxLength(30).HasColumnType("nvarchar(30)");
                b.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
                b.HasKey("Id");
                b.ToTable("ComponentTypes");
                b.HasData(
                    new { Id = 1, Abbreviation = "CPU", Name = "Central Processing Unit" },
                    new { Id = 2, Abbreviation = "GPU", Name = "Graphics Processing Unit" },
                    new { Id = 3, Abbreviation = "RAM", Name = "Random Access Memory" });
            });

            modelBuilder.Entity("ComputerApi.Models.Component", b =>
            {
                b.Property<string>("Code").HasColumnType("char(10)");
                b.Property<int>("ComponentManufacturersId").HasColumnType("int");
                b.Property<int>("ComponentTypesId").HasColumnType("int");
                b.Property<string>("Description").HasColumnType("nvarchar(max)");
                b.Property<string>("Name").IsRequired().HasMaxLength(300).HasColumnType("nvarchar(300)");
                b.HasKey("Code");
                b.HasIndex("ComponentManufacturersId");
                b.HasIndex("ComponentTypesId");
                b.ToTable("Components");
                b.HasData(
                    new { Code = "CPU001    ", Name = "Intel Core i9-13900K", Description = "High-end desktop processor", ComponentManufacturersId = 1, ComponentTypesId = 1 },
                    new { Code = "GPU001    ", Name = "NVIDIA RTX 4090", Description = "Flagship gaming GPU", ComponentManufacturersId = 3, ComponentTypesId = 2 },
                    new { Code = "RAM001    ", Name = "Corsair DDR5 32GB", Description = "High-speed DDR5 memory", ComponentManufacturersId = 2, ComponentTypesId = 3 });
            });

            modelBuilder.Entity("ComputerApi.Models.PC", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));
                b.Property<DateTime>("CreatedAt").HasColumnType("datetime");
                b.Property<string>("Name").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
                b.Property<int>("Stock").HasColumnType("int");
                b.Property<int>("Warranty").HasColumnType("int");
                b.Property<float>("Weight").HasColumnType("real");
                b.HasKey("Id");
                b.ToTable("PCs");
                b.HasData(
                    new { Id = 1, Name = "Gaming Beast X", Weight = 12.5f, Warranty = 36, CreatedAt = new DateTime(2026, 5, 8, 9, 0, 0), Stock = 5 },
                    new { Id = 2, Name = "Office Mini Pro", Weight = 4.2f, Warranty = 24, CreatedAt = new DateTime(2026, 4, 15, 13, 30, 0), Stock = 12 },
                    new { Id = 3, Name = "Workstation Ultra", Weight = 18.0f, Warranty = 48, CreatedAt = new DateTime(2026, 3, 1, 8, 0, 0), Stock = 3 });
            });

            modelBuilder.Entity("ComputerApi.Models.PCComponent", b =>
            {
                b.Property<int>("PCId").HasColumnType("int");
                b.Property<string>("ComponentCode").HasColumnType("char(10)");
                b.Property<int>("Amount").HasColumnType("int");
                b.HasKey("PCId", "ComponentCode");
                b.HasIndex("ComponentCode");
                b.ToTable("PCComponents");
                b.HasData(
                    new { PCId = 1, ComponentCode = "CPU001    ", Amount = 1 },
                    new { PCId = 1, ComponentCode = "GPU001    ", Amount = 2 },
                    new { PCId = 2, ComponentCode = "RAM001    ", Amount = 2 },
                    new { PCId = 3, ComponentCode = "CPU001    ", Amount = 2 },
                    new { PCId = 3, ComponentCode = "GPU001    ", Amount = 4 });
            });

            modelBuilder.Entity("ComputerApi.Models.Component", b =>
            {
                b.HasOne("ComputerApi.Models.ComponentManufacturer", "Manufacturer")
                    .WithMany("Components").HasForeignKey("ComponentManufacturersId")
                    .OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.HasOne("ComputerApi.Models.ComponentType", "ComponentType")
                    .WithMany("Components").HasForeignKey("ComponentTypesId")
                    .OnDelete(DeleteBehavior.Restrict).IsRequired();
                b.Navigation("ComponentType");
                b.Navigation("Manufacturer");
            });

            modelBuilder.Entity("ComputerApi.Models.PCComponent", b =>
            {
                b.HasOne("ComputerApi.Models.Component", "Component")
                    .WithMany("PCComponents").HasForeignKey("ComponentCode")
                    .OnDelete(DeleteBehavior.Cascade).IsRequired();
                b.HasOne("ComputerApi.Models.PC", "PC")
                    .WithMany("PCComponents").HasForeignKey("PCId")
                    .OnDelete(DeleteBehavior.Cascade).IsRequired();
                b.Navigation("Component");
                b.Navigation("PC");
            });

            modelBuilder.Entity("ComputerApi.Models.ComponentManufacturer", b => b.Navigation("Components"));
            modelBuilder.Entity("ComputerApi.Models.ComponentType", b => b.Navigation("Components"));
            modelBuilder.Entity("ComputerApi.Models.Component", b => b.Navigation("PCComponents"));
            modelBuilder.Entity("ComputerApi.Models.PC", b => b.Navigation("PCComponents"));
#pragma warning restore 612, 618
        }
    }
}
