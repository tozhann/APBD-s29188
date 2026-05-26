using Microsoft.EntityFrameworkCore;
using UserPanelApp.Models;

namespace UserPanelApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<UserNote> UserNotes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(160);
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).IsRequired().HasMaxLength(30);
        });

        modelBuilder.Entity<UserNote>(e =>
        {
            e.HasKey(n => n.Id);
            e.Property(n => n.Title).IsRequired().HasMaxLength(160);
            e.Property(n => n.Content).IsRequired().HasMaxLength(2000);

            e.HasOne(n => n.AppUser)
             .WithMany(u => u.Notes)
             .HasForeignKey(n => n.AppUserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed admin user
        // Password: Admin@1234 (hashed with BCrypt)
        modelBuilder.Entity<AppUser>().HasData(new AppUser
        {
            Id           = 1,
            Email        = "admin@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
            Role         = "Admin",
            CreatedAt    = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
