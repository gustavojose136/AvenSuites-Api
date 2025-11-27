using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class RatePlanTests
{
    [Fact]
    public void RatePlan_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var roomTypeId = Guid.NewGuid();

        // Act
        var ratePlan = new RatePlan
        {
            Id = id,
            HotelId = hotelId,
            RoomTypeId = roomTypeId,
            Name = "Flexível",
            Currency = "BRL",
            Active = true
        };

        // Assert
        ratePlan.Id.Should().Be(id);
        ratePlan.HotelId.Should().Be(hotelId);
        ratePlan.RoomTypeId.Should().Be(roomTypeId);
        ratePlan.Name.Should().Be("Flexível");
        ratePlan.Currency.Should().Be("BRL");
        ratePlan.Active.Should().BeTrue();
    }

    [Fact]
    public void RatePlan_ShouldInitializeCollections()
    {
        // Act
        var ratePlan = new RatePlan();

        // Assert
        ratePlan.Prices.Should().NotBeNull();
        ratePlan.Prices.Should().BeEmpty();
    }

    [Fact]
    public void RatePlan_ShouldHaveDefaultCurrency()
    {
        // Act
        var ratePlan = new RatePlan
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            Name = "Test"
        };

        // Assert
        ratePlan.Currency.Should().Be("BRL");
    }
}

