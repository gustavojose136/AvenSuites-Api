using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Application.Utils;
using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Infrastructure.Data.Contexts;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // User Management
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    
    // Hotel Management
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<HotelKey> HotelKeys { get; set; }
    
    // Guests
    public DbSet<Guest> Guests { get; set; }
    public DbSet<GuestPii> GuestPii { get; set; }
    
    // Rooms
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<MaintenanceBlock> MaintenanceBlocks { get; set; }
    
    // Rate Plans
    public DbSet<RatePlan> RatePlans { get; set; }
    public DbSet<RatePlanPrice> RatePlanPrices { get; set; }
    
    // Bookings
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingGuest> BookingGuests { get; set; }
    public DbSet<BookingRoom> BookingRooms { get; set; }
    public DbSet<BookingRoomNight> BookingRoomNights { get; set; }
    public DbSet<BookingPayment> BookingPayments { get; set; }
    public DbSet<BookingStatusHistory> BookingStatusHistories { get; set; }
    
    // Invoices
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<ErpIntegrationLog> ErpIntegrationLogs { get; set; }
    
    // Notifications
    public DbSet<NotificationTemplate> NotificationTemplates { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }
    
    // Chat
    public DbSet<ChatSession> ChatSessions { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    
    // Integration Events
    public DbSet<IntegrationEventOutbox> IntegrationEventOutbox { get; set; }
    public DbSet<IntegrationEventInbox> IntegrationEventInbox { get; set; }
    
    // IPM Integration
    public DbSet<IpmCredentials> IpmCredentials { get; set; }
    
    // Others
    public DbSet<ApiIdempotencyKey> ApiIdempotencyKeys { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

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

        // HotelKey configuration
        modelBuilder.Entity<HotelKey>(entity =>
        {
            entity.HasKey(e => new { e.HotelId, e.KeyVersion });
            
            entity.HasOne(e => e.Hotel)
                .WithMany(e => e.HotelKeys)
                .HasForeignKey(e => e.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ApiIdempotencyKey configuration
        modelBuilder.Entity<ApiIdempotencyKey>(entity =>
        {
            entity.HasKey(e => e.IdempotencyKey);
            entity.HasIndex(e => e.ExpiresAt);
        });

        // BookingGuest configuration (chave composta)
        modelBuilder.Entity<BookingGuest>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.GuestId });
            
            entity.HasOne(e => e.Booking)
                .WithMany(e => e.BookingGuests)
                .HasForeignKey(e => e.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Guest)
                .WithMany(e => e.GuestBookings)
                .HasForeignKey(e => e.GuestId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BookingRoomNight configuration (chave composta única)
        modelBuilder.Entity<BookingRoomNight>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.RoomId, e.StayDate }).IsUnique();
        });

        // GuestPii configuration
        modelBuilder.Entity<GuestPii>(entity =>
        {
            entity.HasKey(e => e.GuestId);
            entity.HasOne(e => e.Guest)
                .WithOne(e => e.GuestPii)
                .HasForeignKey<Guest>(e => e.Id)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.EmailSha256);
            entity.HasIndex(e => e.PhoneSha256);
            entity.HasIndex(e => e.DocumentSha256);
        });

        // NotificationTemplate configuration
        modelBuilder.Entity<NotificationTemplate>(entity =>
        {
            entity.HasKey(e => e.TemplateKey);
        });

        // IpmCredentials configuration
        modelBuilder.Entity<IpmCredentials>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.HotelId).IsUnique();
            
            entity.HasOne(e => e.Hotel)
                .WithOne()
                .HasForeignKey<IpmCredentials>(e => e.HotelId)
                .OnDelete(DeleteBehavior.Restrict);
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

        // Seed Hotel Avenida
        var hotelAvenidaId = Guid.NewGuid();
        modelBuilder.Entity<Hotel>().HasData(
            new Hotel
            {
                Id = hotelAvenidaId,
                Name = "Hotel Avenida",
                TradeName = "Hotel Avenida Suites",
                Cnpj = "12.345.678/0001-90",
                Email = "contato@hotelavenida.com.br",
                PhoneE164 = "+5511999999999",
                Timezone = "America/Sao_Paulo",
                AddressLine1 = "Avenida Paulista, 1234",
                AddressLine2 = "10º Andar",
                City = "São Francisco do Sul",
                State = "SC",
                PostalCode = "01310-100",
                CountryCode = "BR",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed usuário Gustavo para Hotel Avenida
        var gustavoUserId = Guid.NewGuid();
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = gustavoUserId,
                Name = "Gustavo",
                Email = "gjose2980@gmail.com",
                PasswordHash = Argon2PasswordHasher.HashPassword("Admin123!"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            }
        );

        // Assign admin role to Gustavo
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = gustavoUserId,
                RoleId = adminRoleId,
                AssignedAt = DateTime.UtcNow
            }
        );

        // Seed 3 quartos para Hotel Avenida
        var roomTypeId = Guid.NewGuid();
        
        // Criar Room Type para os quartos
        modelBuilder.Entity<RoomType>().HasData(
            new RoomType
            {
                Id = roomTypeId,
                HotelId = hotelAvenidaId,
                Code = "STD",
                Name = "Standard",
                Description = "Quarto padrão com cama de casal",
                CapacityAdults = 2,
                CapacityChildren = 1,
                BasePrice = 150.00m,
                Active = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Criar os 3 quartos
        var room1Id = Guid.NewGuid();
        var room2Id = Guid.NewGuid();
        var room3Id = Guid.NewGuid();

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = room1Id,
                HotelId = hotelAvenidaId,
                RoomTypeId = roomTypeId,
                RoomNumber = "101",
                Floor = "1",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = room2Id,
                HotelId = hotelAvenidaId,
                RoomTypeId = roomTypeId,
                RoomNumber = "102",
                Floor = "1",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Room
            {
                Id = room3Id,
                HotelId = hotelAvenidaId,
                RoomTypeId = roomTypeId,
                RoomNumber = "103",
                Floor = "1",
                Status = "ACTIVE",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
