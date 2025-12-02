using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class GuestRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GuestRepository _guestRepository;

    public GuestRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _guestRepository = new GuestRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingGuest_ShouldReturnGuest()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = "12345678901234"
        };
        _context.Hotels.Add(hotel);

        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            MarketingConsent = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _guestRepository.GetByIdAsync(guest.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(guest.Id);
        result.HotelId.Should().Be(guest.HotelId);
    }

    [Fact]
    public async Task GetByIdWithPiiAsync_WithExistingGuest_ShouldReturnGuestWithPii()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = "12345678901234"
        };
        _context.Hotels.Add(hotel);

        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            MarketingConsent = true,
            CreatedAt = DateTime.UtcNow
        };
        _context.Guests.Add(guest);

        var guestPii = new GuestPii
        {
            GuestId = guest.Id,
            FullName = "Test Guest",
            Email = "guest@test.com",
            DocumentType = "CPF",
            DocumentPlain = "12345678901"
        };
        _context.GuestPii.Add(guestPii);
        await _context.SaveChangesAsync();

        // Act
        var result = await _guestRepository.GetByIdWithPiiAsync(guest.Id);

        // Assert
        result.Should().NotBeNull();
        result!.GuestPii.Should().NotBeNull();
        result.GuestPii!.FullName.Should().Be("Test Guest");
    }

    [Fact]
    public async Task GetByHotelIdAsync_WithExistingGuests_ShouldReturnGuests()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = "12345678901234"
        };
        _context.Hotels.Add(hotel);

        var guest1 = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            CreatedAt = DateTime.UtcNow
        };
        var guest2 = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            CreatedAt = DateTime.UtcNow
        };
        _context.Guests.AddRange(guest1, guest2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _guestRepository.GetByHotelIdAsync(hotel.Id);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAsync_ShouldAddGuest()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = "12345678901234"
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            MarketingConsent = true,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _guestRepository.AddAsync(guest);

        // Assert
        result.Should().NotBeNull();
        var savedGuest = await _context.Guests.FindAsync(guest.Id);
        savedGuest.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateGuest()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = "12345678901234"
        };
        _context.Hotels.Add(hotel);

        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            MarketingConsent = false,
            CreatedAt = DateTime.UtcNow
        };
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        // Act
        guest.MarketingConsent = true;
        guest.UpdatedAt = DateTime.UtcNow;
        await _guestRepository.UpdateAsync(guest);

        // Assert
        var updatedGuest = await _context.Guests.FindAsync(guest.Id);
        updatedGuest!.MarketingConsent.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteGuest()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = "12345678901234"
        };
        _context.Hotels.Add(hotel);

        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            CreatedAt = DateTime.UtcNow
        };
        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        // Act
        await _guestRepository.DeleteAsync(guest.Id);

        // Assert
        var deletedGuest = await _context.Guests.FindAsync(guest.Id);
        deletedGuest.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}


