using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class BookingRoomNightTests
{
    [Fact]
    public void BookingRoomNight_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var bookingRoomId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var stayDate = DateTime.Today;
        var priceAmount = 150.00m;

        // Act
        var bookingRoomNight = new BookingRoomNight
        {
            Id = id,
            BookingRoomId = bookingRoomId,
            RoomId = roomId,
            StayDate = stayDate,
            PriceAmount = priceAmount,
            TaxAmount = 15.00m
        };

        // Assert
        bookingRoomNight.Id.Should().Be(id);
        bookingRoomNight.BookingRoomId.Should().Be(bookingRoomId);
        bookingRoomNight.RoomId.Should().Be(roomId);
        bookingRoomNight.StayDate.Should().Be(stayDate);
        bookingRoomNight.PriceAmount.Should().Be(priceAmount);
        bookingRoomNight.TaxAmount.Should().Be(15.00m);
    }

    [Fact]
    public void BookingRoomNight_ShouldHaveDefaultTaxAmount()
    {
        // Act
        var bookingRoomNight = new BookingRoomNight();

        // Assert
        bookingRoomNight.TaxAmount.Should().Be(0.00m);
    }
}

