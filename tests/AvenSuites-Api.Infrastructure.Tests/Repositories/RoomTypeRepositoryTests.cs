using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class RoomTypeRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RoomTypeRepository _repository;

    public RoomTypeRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new RoomTypeRepository(_context);
    }

    [Fact]
    public async Task GetByIdWithOccupancyPricesAsync_ShouldReturnRoomTypeWithPrices()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var roomTypeId = Guid.NewGuid();
        
        var roomType = new RoomType
        {
            Id = roomTypeId,
            HotelId = hotelId,
            Code = "STD",
            Name = "Standard",
            BasePrice = 150.00m,
            Active = true
        };

        var occupancyPrices = new List<RoomTypeOccupancyPrice>
        {
            new() { Id = Guid.NewGuid(), RoomTypeId = roomTypeId, Occupancy = 1, PricePerNight = 100.00m },
            new() { Id = Guid.NewGuid(), RoomTypeId = roomTypeId, Occupancy = 2, PricePerNight = 150.00m }
        };

        _context.RoomTypes.Add(roomType);
        _context.RoomTypeOccupancyPrices.AddRange(occupancyPrices);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithOccupancyPricesAsync(roomTypeId);

        // Assert
        result.Should().NotBeNull();
        result!.OccupancyPrices.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddAsync_ShouldAddRoomType()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var roomType = new RoomType
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Code = "STD",
            Name = "Standard",
            BasePrice = 150.00m,
            Active = true
        };

        // Act
        var result = await _repository.AddAsync(roomType);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(roomType.Id);
        var saved = await _context.RoomTypes.FindAsync(roomType.Id);
        saved.Should().NotBeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

