using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class RoomRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RoomRepository _repository;

    public RoomRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new RoomRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingRoom_ShouldReturnRoom()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            RoomNumber = "101",
            Floor = "1",
            Status = "ACTIVE"
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(room.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(room.Id);
        result.RoomNumber.Should().Be("101");
    }

    [Fact]
    public async Task GetByIdWithDetailsAsync_ShouldReturnRoomWithRoomType()
    {
        // Arrange
        var roomType = new RoomType
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Code = "STD",
            Name = "Standard",
            BasePrice = 150m
        };

        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = roomType.HotelId,
            RoomTypeId = roomType.Id,
            RoomNumber = "102",
            Status = "ACTIVE"
        };

        _context.RoomTypes.Add(roomType);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithDetailsAsync(room.Id);

        // Assert
        result.Should().NotBeNull();
        result!.RoomType.Should().NotBeNull();
        result.RoomType!.Name.Should().Be("Standard");
    }

    [Fact]
    public async Task GetByHotelIdAsync_ShouldReturnAllRoomsForHotel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var otherHotelId = Guid.NewGuid();

        var rooms = new List<Room>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, RoomTypeId = Guid.NewGuid(), RoomNumber = "101", Status = "ACTIVE" },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, RoomTypeId = Guid.NewGuid(), RoomNumber = "102", Status = "ACTIVE" },
            new() { Id = Guid.NewGuid(), HotelId = otherHotelId, RoomTypeId = Guid.NewGuid(), RoomNumber = "201", Status = "ACTIVE" }
        };

        _context.Rooms.AddRange(rooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByHotelIdAsync(hotelId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(r => r.HotelId.Should().Be(hotelId));
    }

    [Fact]
    public async Task IsRoomAvailableAsync_WithNoConflicts_ShouldReturnTrue()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            RoomNumber = "101",
            Status = "ACTIVE"
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var isAvailable = await _repository.IsRoomAvailableAsync(
            room.Id,
            DateTime.Today.AddDays(10),
            DateTime.Today.AddDays(12));

        // Assert
        isAvailable.Should().BeTrue();
    }

    [Fact]
    public async Task IsRoomAvailableAsync_WithInactiveRoom_ShouldReturnFalse()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            RoomNumber = "101",
            Status = "INACTIVE"
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var isAvailable = await _repository.IsRoomAvailableAsync(
            room.Id,
            DateTime.Today,
            DateTime.Today.AddDays(2));

        // Assert
        isAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_WithValidRoom_ShouldAddRoom()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            RoomNumber = "103",
            Floor = "1",
            Status = "ACTIVE"
        };

        // Act
        var result = await _repository.AddAsync(room);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(room.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedRoom = await _context.Rooms.FindAsync(room.Id);
        savedRoom.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingRoom_ShouldUpdateRoom()
    {
        // Arrange
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            RoomNumber = "101",
            Status = "ACTIVE"
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        room.Status = "MAINTENANCE";
        room.Floor = "2";

        // Act
        var result = await _repository.UpdateAsync(room);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("MAINTENANCE");
        result.Floor.Should().Be("2");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

