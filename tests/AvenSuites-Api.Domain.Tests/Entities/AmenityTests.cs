using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class AmenityTests
{
    [Fact]
    public void Amenity_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var code = "WIFI";
        var name = "Wi-Fi";

        // Act
        var amenity = new Amenity
        {
            Id = id,
            Code = code,
            Name = name
        };

        // Assert
        amenity.Id.Should().Be(id);
        amenity.Code.Should().Be(code);
        amenity.Name.Should().Be(name);
    }

    [Fact]
    public void Amenity_ShouldInitializeCollections()
    {
        // Act
        var amenity = new Amenity();

        // Assert
        amenity.RoomTypes.Should().NotBeNull();
        amenity.RoomTypes.Should().BeEmpty();
    }
}

