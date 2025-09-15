using AvenSuitesApi.Domain.Entities;
using FluentAssertions;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class RoleTests
{
    [Fact]
    public void Role_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Admin";
        var description = "Administrator role";
        var createdAt = DateTime.UtcNow;

        // Act
        var role = new Role
        {
            Id = id,
            Name = name,
            Description = description,
            CreatedAt = createdAt,
            IsActive = true
        };

        // Assert
        role.Id.Should().Be(id);
        role.Name.Should().Be(name);
        role.Description.Should().Be(description);
        role.CreatedAt.Should().Be(createdAt);
        role.IsActive.Should().BeTrue();
        role.UpdatedAt.Should().BeNull();
        role.UserRoles.Should().NotBeNull();
        role.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void Role_ShouldInitializeCollections()
    {
        // Act
        var role = new Role();

        // Assert
        role.UserRoles.Should().NotBeNull();
        role.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void Role_ShouldAllowSettingUpdatedAt()
    {
        // Arrange
        var role = new Role();
        var updatedAt = DateTime.UtcNow;

        // Act
        role.UpdatedAt = updatedAt;

        // Assert
        role.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void Role_ShouldAllowSettingIsActive()
    {
        // Arrange
        var role = new Role();

        // Act
        role.IsActive = false;

        // Assert
        role.IsActive.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Role_ShouldAcceptEmptyOrNullName(string? name)
    {
        // Act
        var role = new Role { Name = name ?? string.Empty };

        // Assert
        role.Name.Should().Be(name ?? string.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Role_ShouldAcceptEmptyOrNullDescription(string? description)
    {
        // Act
        var role = new Role { Description = description };

        // Assert
        role.Description.Should().Be(description);
    }

    [Fact]
    public void Role_ShouldAllowNullDescription()
    {
        // Act
        var role = new Role { Description = null };

        // Assert
        role.Description.Should().BeNull();
    }
}

