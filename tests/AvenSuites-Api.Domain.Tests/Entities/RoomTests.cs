using FluentAssertions;
using AvenSuitesApi.Domain.Entities;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class RoomTests
{
    [Fact]
    public void Room_WithValidData_ShouldBeCreated()
    {
        // Arrange & Act
        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            RoomNumber = "101",
            Floor = "1",
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Assert
        room.Should().NotBeNull();
        room.RoomNumber.Should().Be("101");
        room.Status.Should().Be("ACTIVE");
    }

    [Fact]
    public void Room_WithInactiveStatus_ShouldNotBeActive()
    {
        // Arrange
        var room = new Room
        {
            Status = "INACTIVE"
        };

        // Assert
        room.Status.Should().Be("INACTIVE");
        room.Status.Should().NotBe("ACTIVE");
    }
}

