using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class BookingRoomTests
{
    [Fact]
    public void BookingRoom_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var roomTypeId = Guid.NewGuid();
        var priceTotal = 500.00m;

        // Act
        var bookingRoom = new BookingRoom
        {
            Id = id,
            BookingId = bookingId,
            RoomId = roomId,
            RoomTypeId = roomTypeId,
            PriceTotal = priceTotal
        };

        // Assert
        bookingRoom.Id.Should().Be(id);
        bookingRoom.BookingId.Should().Be(bookingId);
        bookingRoom.RoomId.Should().Be(roomId);
        bookingRoom.RoomTypeId.Should().Be(roomTypeId);
        bookingRoom.PriceTotal.Should().Be(priceTotal);
    }

    [Fact]
    public void BookingRoom_ShouldInitializeCollections()
    {
        // Act
        var bookingRoom = new BookingRoom();

        // Assert
        bookingRoom.Nights.Should().NotBeNull();
        bookingRoom.Nights.Should().BeEmpty();
    }

    [Fact]
    public void BookingRoom_ShouldAllowOptionalProperties()
    {
        // Arrange
        var bookingRoom = new BookingRoom
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            RoomId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid()
        };

        // Act
        bookingRoom.RatePlanId = Guid.NewGuid();
        bookingRoom.Notes = "Quarto com vista para o mar";

        // Assert
        bookingRoom.RatePlanId.Should().NotBeNull();
        bookingRoom.Notes.Should().Be("Quarto com vista para o mar");
    }

    [Fact]
    public void BookingRoom_ShouldHaveDefaultPriceTotal()
    {
        // Act
        var bookingRoom = new BookingRoom();

        // Assert
        bookingRoom.PriceTotal.Should().Be(0.00m);
    }
}

