using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class HotelRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly HotelRepository _hotelRepository;

    public HotelRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _hotelRepository = new HotelRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingHotel_ShouldReturnHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel Test",
            TradeName = "Hotel Test LTDA",
            Cnpj = "12345678000190",
            Email = "test@hotel.com",
            Status = "ACTIVE"
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _hotelRepository.GetByIdAsync(hotel.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(hotel.Id);
        result.Name.Should().Be("Hotel Test");
        result.Cnpj.Should().Be("12345678000190");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingHotel_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _hotelRepository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCnpjAsync_WithExistingCnpj_ShouldReturnHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel Test",
            Cnpj = "12345678000190",
            Status = "ACTIVE"
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _hotelRepository.GetByCnpjAsync("12345678000190");

        // Assert
        result.Should().NotBeNull();
        result!.Cnpj.Should().Be("12345678000190");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveHotels()
    {
        // Arrange
        var activeHotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Active Hotel",
            Cnpj = "11111111000111",
            Status = "ACTIVE"
        };

        var inactiveHotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Inactive Hotel",
            Cnpj = "22222222000222",
            Status = "INACTIVE"
        };

        _context.Hotels.AddRange(activeHotel, inactiveHotel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _hotelRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(h => h.Id == activeHotel.Id);
        result.Should().NotContain(h => h.Id == inactiveHotel.Id);
    }

    [Fact]
    public async Task AddAsync_WithValidHotel_ShouldAddHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "New Hotel",
            Cnpj = "33333333000333",
            Email = "new@hotel.com",
            Status = "ACTIVE"
        };

        // Act
        var result = await _hotelRepository.AddAsync(hotel);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(hotel.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedHotel = await _context.Hotels.FindAsync(hotel.Id);
        savedHotel.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingHotel_ShouldUpdateHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Cnpj = "44444444000444",
            Status = "ACTIVE"
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        hotel.Name = "Updated Name";
        hotel.Email = "updated@hotel.com";

        // Act
        var result = await _hotelRepository.UpdateAsync(hotel);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        result.Email.Should().Be("updated@hotel.com");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteAsync_WithExistingHotel_ShouldSoftDeleteHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel to Delete",
            Cnpj = "55555555000555",
            Status = "ACTIVE"
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        await _hotelRepository.DeleteAsync(hotel.Id);

        // Assert
        var deletedHotel = await _context.Hotels.FindAsync(hotel.Id);
        deletedHotel.Should().NotBeNull();
        deletedHotel!.Status.Should().Be("INACTIVE");
        deletedHotel.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExistsAsync_WithExistingHotel_ShouldReturnTrue()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Existing Hotel",
            Cnpj = "66666666000666",
            Status = "ACTIVE"
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _hotelRepository.ExistsAsync(hotel.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingHotel_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var exists = await _hotelRepository.ExistsAsync(nonExistingId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByCnpjAsync_WithExistingCnpj_ShouldReturnTrue()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel",
            Cnpj = "77777777000777",
            Status = "ACTIVE"
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _hotelRepository.ExistsByCnpjAsync("77777777000777");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByCnpjAsync_WithNonExistingCnpj_ShouldReturnFalse()
    {
        // Arrange & Act
        var exists = await _hotelRepository.ExistsByCnpjAsync("99999999000999");

        // Assert
        exists.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

