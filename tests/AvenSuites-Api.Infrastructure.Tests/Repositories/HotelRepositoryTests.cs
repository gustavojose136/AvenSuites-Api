using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

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
            Name = "Test Hotel",
            Cnpj = "12345678901234",
            CreatedAt = DateTime.UtcNow
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _hotelRepository.GetByIdAsync(hotel.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(hotel.Id);
        result.Name.Should().Be(hotel.Name);
    }

    [Fact]
    public async Task GetByCnpjAsync_WithExistingHotel_ShouldReturnHotel()
    {
        // Arrange
        var cnpj = "12345678901234";
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = cnpj,
            CreatedAt = DateTime.UtcNow
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        var result = await _hotelRepository.GetByCnpjAsync(cnpj);

        // Assert
        result.Should().NotBeNull();
        result!.Cnpj.Should().Be(cnpj);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllHotels()
    {
        // Arrange
        var hotel1 = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel 1",
            Cnpj = "11111111111111",
            CreatedAt = DateTime.UtcNow
        };
        var hotel2 = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel 2",
            Cnpj = "22222222222222",
            CreatedAt = DateTime.UtcNow
        };
        _context.Hotels.AddRange(hotel1, hotel2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _hotelRepository.GetAllAsync();

        // Assert
        result.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task AddAsync_ShouldAddHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "New Hotel",
            Cnpj = "99999999999999",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _hotelRepository.AddAsync(hotel);

        // Assert
        result.Should().NotBeNull();
        var savedHotel = await _context.Hotels.FindAsync(hotel.Id);
        savedHotel.Should().NotBeNull();
        savedHotel!.Name.Should().Be("New Hotel");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Cnpj = "12345678901234",
            CreatedAt = DateTime.UtcNow
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        hotel.Name = "New Name";
        hotel.UpdatedAt = DateTime.UtcNow;
        await _hotelRepository.UpdateAsync(hotel);

        // Assert
        var updatedHotel = await _context.Hotels.FindAsync(hotel.Id);
        updatedHotel!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteHotel()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Cnpj = "12345678901234",
            CreatedAt = DateTime.UtcNow
        };
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // Act
        await _hotelRepository.DeleteAsync(hotel.Id);

        // Assert
        var deletedHotel = await _context.Hotels.FindAsync(hotel.Id);
        deletedHotel.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}



