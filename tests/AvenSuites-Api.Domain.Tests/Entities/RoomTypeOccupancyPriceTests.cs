using FluentAssertions;
using AvenSuitesApi.Domain.Entities;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class RoomTypeOccupancyPriceTests
{
    [Fact]
    public void RoomTypeOccupancyPrice_WithValidData_ShouldBeCreated()
    {
        // Arrange & Act
        var occupancyPrice = new RoomTypeOccupancyPrice
        {
            Id = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            Occupancy = 2,
            PricePerNight = 150.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Assert
        occupancyPrice.Should().NotBeNull();
        occupancyPrice.Occupancy.Should().Be(2);
        occupancyPrice.PricePerNight.Should().Be(150.00m);
    }

    [Fact]
    public void RoomTypeOccupancyPrice_WithDifferentOccupancies_ShouldHaveDifferentPrices()
    {
        // Arrange
        var price1 = new RoomTypeOccupancyPrice { Occupancy = 1, PricePerNight = 100.00m };
        var price2 = new RoomTypeOccupancyPrice { Occupancy = 2, PricePerNight = 150.00m };
        var price3 = new RoomTypeOccupancyPrice { Occupancy = 3, PricePerNight = 200.00m };

        // Assert
        price1.PricePerNight.Should().BeLessThan(price2.PricePerNight);
        price2.PricePerNight.Should().BeLessThan(price3.PricePerNight);
    }
}

