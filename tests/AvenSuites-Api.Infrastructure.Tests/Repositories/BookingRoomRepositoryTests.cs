using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class BookingRoomRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BookingRoomRepository _repository;

    public BookingRoomRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BookingRoomRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingBookingRoom_ShouldReturnBookingRoom()
    {
        // Arrange
        var bookingRoom = new BookingRoom
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            PriceTotal = 500m
        };

        _context.BookingRooms.Add(bookingRoom);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(bookingRoom.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(bookingRoom.Id);
        result.PriceTotal.Should().Be(500m);
    }

    [Fact]
    public async Task GetByBookingIdAsync_ShouldReturnAllBookingRoomsForBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var otherBookingId = Guid.NewGuid();

        var bookingRooms = new List<BookingRoom>
        {
            new() { Id = Guid.NewGuid(), BookingId = bookingId, RoomId = Guid.NewGuid(), RoomTypeId = Guid.NewGuid(), PriceTotal = 300m },
            new() { Id = Guid.NewGuid(), BookingId = bookingId, RoomId = Guid.NewGuid(), RoomTypeId = Guid.NewGuid(), PriceTotal = 400m },
            new() { Id = Guid.NewGuid(), BookingId = otherBookingId, RoomId = Guid.NewGuid(), RoomTypeId = Guid.NewGuid(), PriceTotal = 500m }
        };

        _context.BookingRooms.AddRange(bookingRooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBookingIdAsync(bookingId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(br => br.BookingId.Should().Be(bookingId));
    }

    [Fact]
    public async Task GetByRoomIdAsync_ShouldReturnAllBookingRoomsForRoom()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var otherRoomId = Guid.NewGuid();

        var bookingRooms = new List<BookingRoom>
        {
            new() { Id = Guid.NewGuid(), BookingId = Guid.NewGuid(), RoomId = roomId, RoomTypeId = Guid.NewGuid(), PriceTotal = 300m },
            new() { Id = Guid.NewGuid(), BookingId = Guid.NewGuid(), RoomId = roomId, RoomTypeId = Guid.NewGuid(), PriceTotal = 400m },
            new() { Id = Guid.NewGuid(), BookingId = Guid.NewGuid(), RoomId = otherRoomId, RoomTypeId = Guid.NewGuid(), PriceTotal = 500m }
        };

        _context.BookingRooms.AddRange(bookingRooms);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByRoomIdAsync(roomId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(br => br.RoomId.Should().Be(roomId));
    }

    [Fact]
    public async Task AddAsync_WithValidBookingRoom_ShouldAddBookingRoom()
    {
        // Arrange
        var bookingRoom = new BookingRoom
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            PriceTotal = 600m
        };

        // Act
        var result = await _repository.AddAsync(bookingRoom);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(bookingRoom.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedBookingRoom = await _context.BookingRooms.FindAsync(bookingRoom.Id);
        savedBookingRoom.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingBookingRoom_ShouldUpdateBookingRoom()
    {
        // Arrange
        var bookingRoom = new BookingRoom
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            PriceTotal = 500m
        };

        _context.BookingRooms.Add(bookingRoom);
        await _context.SaveChangesAsync();

        bookingRoom.PriceTotal = 700m;
        bookingRoom.Notes = "Quarto com vista para o mar";

        // Act
        var result = await _repository.UpdateAsync(bookingRoom);

        // Assert
        result.Should().NotBeNull();
        result.PriceTotal.Should().Be(700m);
        result.Notes.Should().Be("Quarto com vista para o mar");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExistsAsync_WithExistingBookingRoom_ShouldReturnTrue()
    {
        // Arrange
        var bookingRoom = new BookingRoom
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            PriceTotal = 500m
        };

        _context.BookingRooms.Add(bookingRoom);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsAsync(bookingRoom.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingBookingRoom_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var exists = await _repository.ExistsAsync(nonExistingId);

        // Assert
        exists.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

