using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class GuestTests
{
    [Fact]
    public void Guest_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Act
        var guest = new Guest
        {
            Id = id,
            HotelId = hotelId,
            UserId = userId,
            MarketingConsent = true
        };

        // Assert
        guest.Id.Should().Be(id);
        guest.HotelId.Should().Be(hotelId);
        guest.UserId.Should().Be(userId);
        guest.MarketingConsent.Should().BeTrue();
    }

    [Fact]
    public void Guest_ShouldInitializeCollections()
    {
        // Act
        var guest = new Guest();

        // Assert
        guest.Bookings.Should().NotBeNull();
        guest.Bookings.Should().BeEmpty();
        guest.GuestBookings.Should().NotBeNull();
        guest.GuestBookings.Should().BeEmpty();
    }

    [Fact]
    public void Guest_ShouldAllowOptionalUserId()
    {
        // Arrange
        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid()
        };

        // Act & Assert
        guest.UserId.Should().BeNull();
        guest.UserId = Guid.NewGuid();
        guest.UserId.Should().NotBeNull();
    }

    [Fact]
    public void Guest_ShouldAllowSettingMarketingConsent()
    {
        // Arrange
        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid()
        };

        // Act
        guest.MarketingConsent = false;

        // Assert
        guest.MarketingConsent.Should().BeFalse();
    }
}

