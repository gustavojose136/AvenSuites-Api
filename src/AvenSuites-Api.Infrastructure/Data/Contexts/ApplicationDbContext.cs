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
            entity.Property(e => e.HotelId).IsRequired(false); // Explicitamente nullable
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.IsActive).IsRequired();

            entity.HasIndex(e => e.Email).IsUnique();
            
            // Relacionamento com Hotel
            entity.HasOne(e => e.Hotel)
                .WithMany()
                .HasForeignKey(e => e.HotelId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
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

        // NotificationLog configuration
        modelBuilder.Entity<NotificationLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Template)
                .WithMany(t => t.NotificationLogs)
                .HasForeignKey(e => e.TemplateKey)
                .OnDelete(DeleteBehavior.SetNull);
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
        // Valores fixos para seed (não usar Guid.NewGuid() ou DateTime.UtcNow)
        var fixedCreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // Seed roles
        var adminRoleId = new Guid("60ccaec1-6c42-4fb5-a104-2036b42585a3");
        var hotelAdminRoleId = new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        var userRoleId = new Guid("27648377-84b3-44ef-b9b0-45c9cd8fd9fc");

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Description = "Administrator role with full access to all hotels",
                CreatedAt = fixedCreatedAt,
                IsActive = true
            },
            new Role
            {
                Id = hotelAdminRoleId,
                Name = "Hotel-Admin",
                Description = "Hotel administrator role with access to specific hotel only",
                CreatedAt = fixedCreatedAt,
                IsActive = true
            },
            new Role
            {
                Id = userRoleId,
                Name = "User",
                Description = "Standard user role",
                CreatedAt = fixedCreatedAt,
                IsActive = true
            }
        );

        // Seed admin user
        var adminUserId = new Guid("2975cf19-0baa-4507-9f98-968760deb546");
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                Name = "Administrator",
                Email = "admin@avensuites.com",
                PasswordHash = Argon2PasswordHasher.HashPassword("Admin123!"),
                CreatedAt = fixedCreatedAt,
                IsActive = true
            }
        );

        // Assign admin role to admin user
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = adminUserId,
                RoleId = adminRoleId,
                AssignedAt = fixedCreatedAt
            }
        );

        // Seed Hotel Avenida
        var hotelAvenidaId = new Guid("7a326969-3bf6-40d9-96dc-1aecef585000");
        modelBuilder.Entity<Hotel>().HasData(
            new Hotel
            {
                Id = hotelAvenidaId,
                Name = "Hotel Avenida",
                TradeName = "Hotel Avenida",
                Cnpj = "83.630.657/0001-60",
                Email = "gjose2980@gmail.com",
                PhoneE164 = "+554799662998",
                Timezone = "America/Sao_Paulo",
                AddressLine1 = "Av. Dr. Nereu Ramos, 474",
                AddressLine2 = "Rocio Grande, São Francisco do Sul - SC",
                City = "São Francisco do Sul",
                State = "SC",
                PostalCode = "89331-260",
                CountryCode = "BR",
                Status = "ACTIVE",
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            }
        );

        // Seed usuário Gustavo para Hotel Avenida
        var gustavoUserId = new Guid("f36d8acd-1822-4019-ac76-a6ea959d5193");
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = gustavoUserId,
                Name = "Gustavo",
                Email = "gjose2980@gmail.com",
                PasswordHash = Argon2PasswordHasher.HashPassword("Admin123!"),
                HotelId = hotelAvenidaId, // Associar ao Hotel Avenida
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt,
                IsActive = true
            }
        );

        // Assign Hotel-Admin role to Gustavo (admin do Hotel Avenida)
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                UserId = gustavoUserId,
                RoleId = hotelAdminRoleId,
                AssignedAt = fixedCreatedAt
            }
        );

        // Seed 3 quartos para Hotel Avenida
        var roomTypeId = new Guid("2318702e-1c6d-4d1c-8f07-d6e0ace9d441");
        var roomTypeId2 = new Guid("e9e7976d-59fd-4bda-9468-4d5fdb6feec5");
        
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
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            },
             new RoomType
            {
                Id = roomTypeId2,
                HotelId = hotelAvenidaId,
                Code = "BSC",
                Name = "Basic",
                Description = "Quarto básico com cama de casal",
                CapacityAdults = 1,
                CapacityChildren = 0,
                BasePrice = 130.00m,
                Active = true,
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            }
        );

        // Criar os 3 quartos
        var room1Id = new Guid("40d5718c-dbda-40c7-a4f4-644cd6f177bd");
        var room2Id = new Guid("4cdcf044-587e-4047-b164-a8cd64bad303");
        var room3Id = new Guid("6bd29bd5-4826-45a0-b734-3197fec5cfbd");
        var room4Id = new Guid("bd823cb6-d7a4-45ae-9853-66895ea593bb");

        modelBuilder.Entity<Room>().HasData(
            new Room
            {
                Id = room1Id,
                HotelId = hotelAvenidaId,
                RoomTypeId = roomTypeId,
                RoomNumber = "101",
                Floor = "1",
                Status = "ACTIVE",
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            },
            new Room
            {
                Id = room2Id,
                HotelId = hotelAvenidaId,
                RoomTypeId = roomTypeId,
                RoomNumber = "102",
                Floor = "1",
                Status = "ACTIVE",
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            },
            new Room
            {
                Id = room3Id,
                HotelId = hotelAvenidaId,
                RoomTypeId = roomTypeId,
                RoomNumber = "103",
                Floor = "1",
                Status = "ACTIVE",
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            },
            new Room
            {
                Id = room4Id,
                HotelId = hotelAvenidaId,
                RoomTypeId = roomTypeId2,
                RoomNumber = "11",
                Floor = "1",
                Status = "ACTIVE",
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            }
        );

        // Seed IPM Credentials para Hotel Avenida
        // IMPORTANTE: A senha deve ser criptografada antes de salvar no seed
        // A chave de criptografia deve ser a mesma configurada no appsettings.json
        var ipmCredentialsId = new Guid("0891eb4a-28ae-46bd-8a77-2c2047c54716");
        // Usar a mesma chave padrão do appsettings.json: "AvenSuites-Encryption-Key-32-Chars!!"
        var encryptedPassword = EncryptForSeed("@Gu0304.", "AvenSuites-Encryption-Key-32-Chars!!");
        
        modelBuilder.Entity<IpmCredentials>().HasData(
            new IpmCredentials
            {
                Id = ipmCredentialsId,
                HotelId = hotelAvenidaId,
                Username = "83.630.657/0001-60",
                Password = encryptedPassword, // Senha criptografada
                CpfCnpj = "83.630.657/0001-60",
                CityCode = "8319",
                SerieNfse = "1",
                Active = true,
                CreatedAt = fixedCreatedAt,
                UpdatedAt = fixedCreatedAt
            }
        );

        // Seed Guest - Joni Cardoso para Hotel Avenida
        var joniGuestId = new Guid("87f086dd-d461-49c8-a63c-1fc7b6a55441");
        var joniCreatedAt = fixedCreatedAt;
        
        modelBuilder.Entity<Guest>().HasData(
            new Guest
            {
                Id = joniGuestId,
                HotelId = hotelAvenidaId,
                MarketingConsent = false,
                CreatedAt = joniCreatedAt,
                UpdatedAt = joniCreatedAt
            }
        );
        var cpfHash = ComputeSha256Hash("791.300.709-53");
        
        modelBuilder.Entity<GuestPii>().HasData(
            new GuestPii
            {
                GuestId = joniGuestId,
                FullName = "Joni Cardoso",
                Email = null, // Não informado
                EmailSha256 = null,
                PhoneE164 = null, // Não informado
                PhoneSha256 = null,
                DocumentType = "CPF",
                DocumentPlain = "791.300.709-53",
                DocumentSha256 = cpfHash,
                BirthDate = null,
                AddressLine1 = "MONSENHOR GERCINO, S/N",
                AddressLine2 = "NÃO INFORMADO",
                City = "Joinville",
                Neighborhood = "JARIVATUBA",
                State = "SC",
                PostalCode = "89230-290",
                CountryCode = "BR",
                DocumentCipher = null, // Em produção, seria criptografado
                DocumentNonce = null,
                DocumentTag = null,
                DocumentKeyVersion = 1,
                CreatedAt = joniCreatedAt,
                UpdatedAt = joniCreatedAt
            }
        );
    }

    /// <summary>
    /// Método helper para criptografar senha no seed.
    /// Usa a mesma lógica do SecureEncryptionService para manter consistência.
    /// </summary>
    private static string EncryptForSeed(string plainText, string encryptionKey)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        // Garantir que a chave tenha exatamente 32 bytes
        var keyBytes = System.Text.Encoding.UTF8.GetBytes(encryptionKey);
        var key = new byte[32];
        var copyLength = Math.Min(keyBytes.Length, 32);
        Array.Copy(keyBytes, 0, key, 0, copyLength);

        using var aes = System.Security.Cryptography.Aes.Create();
        aes.Key = key;
        aes.Mode = System.Security.Cryptography.CipherMode.CBC;
        aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        using var msEncrypt = new MemoryStream();
        
        // Escrever IV primeiro (16 bytes)
        msEncrypt.Write(aes.IV, 0, aes.IV.Length);

        using (var csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
        using (var swEncrypt = new StreamWriter(csEncrypt))
        {
            swEncrypt.Write(plainText);
        }

        var encrypted = msEncrypt.ToArray();
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// Calcula hash SHA256 de uma string para busca em dados PII
    /// </summary>
    private static string ComputeSha256Hash(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
