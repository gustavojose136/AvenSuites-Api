using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class BookingRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly BookingRepository _repository;

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BookingRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingBooking_ShouldReturnBooking()
    {
        // Arrange
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            Status = "PENDING",
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL",
            Adults = 2,
            Children = 0
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(booking.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(booking.Id);
        result.Code.Should().Be("RES-001");
        result.Status.Should().Be("PENDING");
    }

    [Fact]
    public async Task GetByCodeAsync_WithExistingCode_ShouldReturnBooking()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Code = "RES-002",
            Status = "CONFIRMED",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL"
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByCodeAsync(hotelId, "RES-002");

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("RES-002");
    }

    [Fact]
    public async Task GetByHotelIdAsync_ShouldReturnAllBookingsForHotel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var otherHotelId = Guid.NewGuid();

        var bookings = new List<Booking>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, Code = "RES-001", Status = "PENDING", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1), MainGuestId = Guid.NewGuid(), Currency = "BRL" },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, Code = "RES-002", Status = "CONFIRMED", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1), MainGuestId = Guid.NewGuid(), Currency = "BRL" },
            new() { Id = Guid.NewGuid(), HotelId = otherHotelId, Code = "RES-003", Status = "PENDING", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(1), MainGuestId = Guid.NewGuid(), Currency = "BRL" }
        };

        _context.Bookings.AddRange(bookings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByHotelIdAsync(hotelId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(b => b.HotelId.Should().Be(hotelId));
    }

    [Fact]
    public async Task AddAsync_WithValidBooking_ShouldAddBooking()
    {
        // Arrange
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Code = "RES-NEW",
            Status = "PENDING",
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL"
        };

        // Act
        var result = await _repository.AddAsync(booking);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(booking.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedBooking = await _context.Bookings.FindAsync(booking.Id);
        savedBooking.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingBooking_ShouldUpdateBooking()
    {
        // Arrange
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            Status = "PENDING",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL"
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        booking.Status = "CONFIRMED";
        booking.Notes = "Updated notes";

        // Act
        var result = await _repository.UpdateAsync(booking);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("CONFIRMED");
        result.Notes.Should().Be("Updated notes");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

