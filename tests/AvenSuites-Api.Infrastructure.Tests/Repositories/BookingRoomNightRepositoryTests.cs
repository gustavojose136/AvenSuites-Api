using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class BookingRoomNightRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BookingRoomNightRepository _repository;

    public BookingRoomNightRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BookingRoomNightRepository(_context);
    }

    [Fact]
    public async Task GetByBookingRoomIdAsync_WithExistingNights_ShouldReturnNights()
    {
        // Arrange
        var bookingRoomId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        
        var nights = new List<BookingRoomNight>
        {
            new() { Id = Guid.NewGuid(), BookingRoomId = bookingRoomId, RoomId = roomId, StayDate = DateTime.Today, PriceAmount = 100m },
            new() { Id = Guid.NewGuid(), BookingRoomId = bookingRoomId, RoomId = roomId, StayDate = DateTime.Today.AddDays(1), PriceAmount = 100m }
        };

        _context.BookingRoomNights.AddRange(nights);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBookingRoomIdAsync(bookingRoomId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(n => n.BookingRoomId.Should().Be(bookingRoomId));
    }

    [Fact]
    public async Task HasConflictAsync_WithConflictingDates_ShouldReturnTrue()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var bookingRoomId = Guid.NewGuid();
        
        var booking = new Booking
        {
            Id = bookingId,
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            Status = "CONFIRMED",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL"
        };

        var bookingRoom = new BookingRoom
        {
            Id = bookingRoomId,
            BookingId = bookingId,
            RoomId = roomId,
            RoomTypeId = Guid.NewGuid(),
            PriceTotal = 200m
        };

        var night = new BookingRoomNight
        {
            Id = Guid.NewGuid(),
            BookingRoomId = bookingRoomId,
            RoomId = roomId,
            StayDate = DateTime.Today.AddDays(1),
            PriceAmount = 100m
        };

        _context.Bookings.Add(booking);
        _context.BookingRooms.Add(bookingRoom);
        _context.BookingRoomNights.Add(night);
        await _context.SaveChangesAsync();

        // Act
        var hasConflict = await _repository.HasConflictAsync(
            roomId, 
            DateTime.Today, 
            DateTime.Today.AddDays(3));

        // Assert
        hasConflict.Should().BeTrue();
    }

    [Fact]
    public async Task HasConflictAsync_WithCancelledBooking_ShouldReturnFalse()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var bookingRoomId = Guid.NewGuid();
        
        var booking = new Booking
        {
            Id = bookingId,
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            Status = "CANCELLED",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL"
        };

        var bookingRoom = new BookingRoom
        {
            Id = bookingRoomId,
            BookingId = bookingId,
            RoomId = roomId,
            RoomTypeId = Guid.NewGuid(),
            PriceTotal = 200m
        };

        var night = new BookingRoomNight
        {
            Id = Guid.NewGuid(),
            BookingRoomId = bookingRoomId,
            RoomId = roomId,
            StayDate = DateTime.Today,
            PriceAmount = 100m
        };

        _context.Bookings.Add(booking);
        _context.BookingRooms.Add(bookingRoom);
        _context.BookingRoomNights.Add(night);
        await _context.SaveChangesAsync();

        // Act
        var hasConflict = await _repository.HasConflictAsync(
            roomId, 
            DateTime.Today, 
            DateTime.Today.AddDays(3));

        // Assert
        hasConflict.Should().BeFalse();
    }

    [Fact]
    public async Task AddRangeAsync_WithMultipleNights_ShouldAddAllNights()
    {
        // Arrange
        var nights = new List<BookingRoomNight>
        {
            new() { Id = Guid.NewGuid(), BookingRoomId = Guid.NewGuid(), RoomId = Guid.NewGuid(), StayDate = DateTime.Today, PriceAmount = 100m },
            new() { Id = Guid.NewGuid(), BookingRoomId = Guid.NewGuid(), RoomId = Guid.NewGuid(), StayDate = DateTime.Today.AddDays(1), PriceAmount = 100m }
        };

        // Act
        await _repository.AddRangeAsync(nights);

        // Assert
        var savedNights = await _context.BookingRoomNights.ToListAsync();
        savedNights.Should().HaveCount(2);
        savedNights.Should().AllSatisfy(n => n.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task DeleteByBookingRoomIdAsync_WithExistingNights_ShouldDeleteAllNights()
    {
        // Arrange
        var bookingRoomId = Guid.NewGuid();
        var nights = new List<BookingRoomNight>
        {
            new() { Id = Guid.NewGuid(), BookingRoomId = bookingRoomId, RoomId = Guid.NewGuid(), StayDate = DateTime.Today, PriceAmount = 100m },
            new() { Id = Guid.NewGuid(), BookingRoomId = bookingRoomId, RoomId = Guid.NewGuid(), StayDate = DateTime.Today.AddDays(1), PriceAmount = 100m }
        };

        _context.BookingRoomNights.AddRange(nights);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteByBookingRoomIdAsync(bookingRoomId);

        // Assert
        var remainingNights = await _context.BookingRoomNights
            .Where(n => n.BookingRoomId == bookingRoomId)
            .ToListAsync();
        remainingNights.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
