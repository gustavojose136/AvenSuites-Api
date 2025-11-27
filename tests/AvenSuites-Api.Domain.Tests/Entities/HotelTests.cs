using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class HotelTests
{
    [Fact]
    public void Hotel_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Hotel Avenida";
        var cnpj = "12345678000190";

        // Act
        var hotel = new Hotel
        {
            Id = id,
            Name = name,
            Cnpj = cnpj,
            Status = "ACTIVE",
            Timezone = "America/Sao_Paulo",
            CountryCode = "BR"
        };

        // Assert
        hotel.Id.Should().Be(id);
        hotel.Name.Should().Be(name);
        hotel.Cnpj.Should().Be(cnpj);
        hotel.Status.Should().Be("ACTIVE");
        hotel.Timezone.Should().Be("America/Sao_Paulo");
        hotel.CountryCode.Should().Be("BR");
    }

    [Fact]
    public void Hotel_ShouldInitializeCollections()
    {
        // Act
        var hotel = new Hotel();

        // Assert
        hotel.Users.Should().NotBeNull();
        hotel.Users.Should().BeEmpty();
        hotel.Guests.Should().NotBeNull();
        hotel.Guests.Should().BeEmpty();
        hotel.RoomTypes.Should().NotBeNull();
        hotel.RoomTypes.Should().BeEmpty();
        hotel.Rooms.Should().NotBeNull();
        hotel.Rooms.Should().BeEmpty();
        hotel.Bookings.Should().NotBeNull();
        hotel.Bookings.Should().BeEmpty();
    }

    [Fact]
    public void Hotel_ShouldAllowSettingOptionalProperties()
    {
        // Arrange
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel Test"
        };

        // Act
        hotel.TradeName = "Hotel Test LTDA";
        hotel.Email = "test@hotel.com";
        hotel.PhoneE164 = "+5511999999999";
        hotel.AddressLine1 = "Rua Teste, 123";
        hotel.City = "São Paulo";
        hotel.State = "SP";
        hotel.PostalCode = "01234-567";

        // Assert
        hotel.TradeName.Should().Be("Hotel Test LTDA");
        hotel.Email.Should().Be("test@hotel.com");
        hotel.PhoneE164.Should().Be("+5511999999999");
        hotel.AddressLine1.Should().Be("Rua Teste, 123");
        hotel.City.Should().Be("São Paulo");
        hotel.State.Should().Be("SP");
        hotel.PostalCode.Should().Be("01234-567");
    }

    [Fact]
    public void Hotel_ShouldHaveDefaultValues()
    {
        // Act
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel Test"
        };

        // Assert
        hotel.Status.Should().Be("ACTIVE");
        hotel.Timezone.Should().Be("America/Sao_Paulo");
        hotel.CountryCode.Should().Be("BR");
    }
}

