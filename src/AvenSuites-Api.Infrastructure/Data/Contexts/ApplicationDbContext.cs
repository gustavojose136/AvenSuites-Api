using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Application.Utils;
using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Infrastructure.Data.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Role configuration
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // UserRole configuration
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
            entity.Property(e => e.AssignedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Role)
                .WithMany(e => e.UserRoles)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed roles
        var adminRoleId = Guid.NewGuid();
        var userRoleId = Guid.NewGuid();

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Description = "Administrator role with full access",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            },
            new Role
            {
                Id = userRoleId,
                Name = "User",
                Description = "Standard user role",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );

        // Seed admin user
        var adminUserId = Guid.NewGuid();
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                Name = "Administrator",
                Email = "admin@avensuites.com",
                PasswordHash = Argon2PasswordHasher.HashPassword("Admin123!"),
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );

        // Assign admin role to admin user
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = adminUserId,
                RoleId = adminRoleId,
                AssignedAt = DateTime.UtcNow
            }
        );
    }
}
