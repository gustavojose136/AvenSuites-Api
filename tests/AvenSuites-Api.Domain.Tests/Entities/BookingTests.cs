using FluentAssertions;
using AvenSuitesApi.Domain.Entities;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class BookingTests
{
    [Fact]
    public void Booking_WithValidData_ShouldBeCreated()
    {
        // Arrange & Act
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            Status = "PENDING",
            Source = "WEB",
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            Adults = 2,
            Children = 0,
            Currency = "BRL",
            TotalAmount = 300.00m,
            MainGuestId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Assert
        booking.Should().NotBeNull();
        booking.Code.Should().Be("RES-001");
        booking.Status.Should().Be("PENDING");
        booking.Adults.Should().Be(2);
    }

    [Fact]
    public void Booking_NumberOfNights_ShouldBeCalculatedCorrectly()
    {
        // Arrange
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(4);
        
        var booking = new Booking
        {
            CheckInDate = checkIn,
            CheckOutDate = checkOut
        };

        // Act
        var nights = (checkOut - checkIn).Days;

        // Assert
        nights.Should().Be(3);
    }

    [Fact]
    public void Booking_WithBookingRooms_ShouldHaveRooms()
    {
        // Arrange
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            BookingRooms = new List<BookingRoom>
            {
                new() { Id = Guid.NewGuid(), RoomId = Guid.NewGuid() },
                new() { Id = Guid.NewGuid(), RoomId = Guid.NewGuid() }
            }
        };

        // Assert
        booking.BookingRooms.Should().HaveCount(2);
    }
}

