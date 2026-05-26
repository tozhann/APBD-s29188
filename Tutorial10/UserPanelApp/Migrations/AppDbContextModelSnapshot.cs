using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using UserPanelApp.Data;

#nullable disable

namespace UserPanelApp.Migrations
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

            Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal
                .SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("UserPanelApp.Models.AppUser", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
                b.Property<string>("Email").IsRequired().HasMaxLength(160).HasColumnType("nvarchar(160)");
                b.Property<string>("PasswordHash").IsRequired().HasColumnType("nvarchar(max)");
                b.Property<string>("Role").IsRequired().HasMaxLength(30).HasColumnType("nvarchar(30)");
                b.HasKey("Id");
                b.HasIndex("Email").IsUnique();
                b.ToTable("AppUsers");
            });

            modelBuilder.Entity("UserPanelApp.Models.UserNote", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
                b.Property<int>("AppUserId").HasColumnType("int");
                b.Property<string>("Content").IsRequired().HasMaxLength(2000).HasColumnType("nvarchar(2000)");
                b.Property<DateTime>("CreatedAt").HasColumnType("datetime2");
                b.Property<string>("Title").IsRequired().HasMaxLength(160).HasColumnType("nvarchar(160)");
                b.HasKey("Id");
                b.HasIndex("AppUserId");
                b.ToTable("UserNotes");
            });

            modelBuilder.Entity("UserPanelApp.Models.UserNote", b =>
            {
                b.HasOne("UserPanelApp.Models.AppUser", "AppUser")
                    .WithMany("Notes")
                    .HasForeignKey("AppUserId")
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
                b.Navigation("AppUser");
            });

            modelBuilder.Entity("UserPanelApp.Models.AppUser", b => b.Navigation("Notes"));
#pragma warning restore 612, 618
        }
    }
}
