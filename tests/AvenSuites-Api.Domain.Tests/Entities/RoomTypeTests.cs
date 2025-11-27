using FluentAssertions;
using AvenSuitesApi.Domain.Entities;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class RoomTypeTests
{
    [Fact]
    public void RoomType_WithValidData_ShouldBeCreated()
    {
        // Arrange & Act
        var roomType = new RoomType
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Code = "STD",
            Name = "Standard",
            Description = "Quarto padrão",
            CapacityAdults = 2,
            CapacityChildren = 1,
            BasePrice = 150.00m,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Assert
        roomType.Should().NotBeNull();
        roomType.Code.Should().Be("STD");
        roomType.Name.Should().Be("Standard");
        roomType.BasePrice.Should().Be(150.00m);
    }

    [Fact]
    public void RoomType_WithOccupancyPrices_ShouldHavePrices()
    {
        // Arrange
        var roomType = new RoomType
        {
            Id = Guid.NewGuid(),
            OccupancyPrices = new List<RoomTypeOccupancyPrice>
            {
                new() { Occupancy = 1, PricePerNight = 100.00m },
                new() { Occupancy = 2, PricePerNight = 150.00m },
                new() { Occupancy = 3, PricePerNight = 200.00m }
            }
        };

        // Assert
        roomType.OccupancyPrices.Should().HaveCount(3);
        roomType.OccupancyPrices.First(op => op.Occupancy == 2).PricePerNight.Should().Be(150.00m);
    }

    [Fact]
    public void RoomType_TotalCapacity_ShouldBeAdultsPlusChildren()
    {
        // Arrange
        var roomType = new RoomType
        {
            CapacityAdults = 2,
            CapacityChildren = 1
        };

        // Act
        var totalCapacity = roomType.CapacityAdults + roomType.CapacityChildren;

        // Assert
        totalCapacity.Should().Be(3);
    }
}

