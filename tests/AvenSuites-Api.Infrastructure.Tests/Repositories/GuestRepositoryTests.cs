using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class GuestRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly GuestRepository _repository;

    public GuestRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new GuestRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingGuest_ShouldReturnGuest()
    {
        // Arrange
        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            MarketingConsent = true
        };

        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(guest.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(guest.Id);
        result.HotelId.Should().Be(guest.HotelId);
    }

    [Fact]
    public async Task GetByUserId_WithExistingUser_ShouldReturnGuest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            UserId = userId,
            MarketingConsent = true
        };

        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserId(userId);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
    }

    [Fact]
    public async Task GetByIdWithPiiAsync_WithExistingGuest_ShouldReturnGuestWithPii()
    {
        // Arrange
        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            MarketingConsent = true,
            GuestPii = new GuestPii
            {
                GuestId = Guid.NewGuid(),
                FullName = "João Silva",
                Email = "joao@email.com"
            }
        };

        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithPiiAsync(guest.Id);

        // Assert
        result.Should().NotBeNull();
        result!.GuestPii.Should().NotBeNull();
        result.GuestPii!.FullName.Should().Be("João Silva");
    }

    [Fact]
    public async Task GetByHotelIdAsync_ShouldReturnAllGuestsForHotel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var otherHotelId = Guid.NewGuid();

        var guests = new List<Guest>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, UserId = Guid.NewGuid(), MarketingConsent = true },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, UserId = Guid.NewGuid(), MarketingConsent = false },
            new() { Id = Guid.NewGuid(), HotelId = otherHotelId, UserId = Guid.NewGuid(), MarketingConsent = true }
        };

        _context.Guests.AddRange(guests);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByHotelIdAsync(hotelId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(g => g.HotelId.Should().Be(hotelId));
    }

    [Fact]
    public async Task AddAsync_WithValidGuest_ShouldAddGuest()
    {
        // Arrange
        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            MarketingConsent = true
        };

        // Act
        var result = await _repository.AddAsync(guest);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(guest.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedGuest = await _context.Guests.FindAsync(guest.Id);
        savedGuest.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingGuest_ShouldUpdateGuest()
    {
        // Arrange
        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            MarketingConsent = true
        };

        _context.Guests.Add(guest);
        await _context.SaveChangesAsync();

        guest.MarketingConsent = false;

        // Act
        var result = await _repository.UpdateAsync(guest);

        // Assert
        result.Should().NotBeNull();
        result.MarketingConsent.Should().BeFalse();
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

