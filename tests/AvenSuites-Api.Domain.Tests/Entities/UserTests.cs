using AvenSuitesApi.Domain.Entities;
using FluentAssertions;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "João Silva";
        var email = "joao@email.com";
        var passwordHash = "hashed_password";
        var createdAt = DateTime.UtcNow;

        // Act
        var user = new User
        {
            Id = id,
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = createdAt,
            IsActive = true
        };

        // Assert
        user.Id.Should().Be(id);
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.CreatedAt.Should().Be(createdAt);
        user.IsActive.Should().BeTrue();
        user.UpdatedAt.Should().BeNull();
        user.UserRoles.Should().NotBeNull();
        user.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void User_ShouldInitializeCollections()
    {
        // Act
        var user = new User();

        // Assert
        user.UserRoles.Should().NotBeNull();
        user.UserRoles.Should().BeEmpty();
    }

    [Fact]
    public void User_ShouldAllowSettingUpdatedAt()
    {
        // Arrange
        var user = new User();
        var updatedAt = DateTime.UtcNow;

        // Act
        user.UpdatedAt = updatedAt;

        // Assert
        user.UpdatedAt.Should().Be(updatedAt);
    }

    [Fact]
    public void User_ShouldAllowSettingIsActive()
    {
        // Arrange
        var user = new User();

        // Act
        user.IsActive = false;

        // Assert
        user.IsActive.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void User_ShouldAcceptEmptyOrNullName(string? name)
    {
        // Act
        var user = new User { Name = name ?? string.Empty };

        // Assert
        user.Name.Should().Be(name ?? string.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void User_ShouldAcceptEmptyOrNullEmail(string? email)
    {
        // Act
        var user = new User { Email = email ?? string.Empty };

        // Assert
        user.Email.Should().Be(email ?? string.Empty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void User_ShouldAcceptEmptyOrNullPasswordHash(string? passwordHash)
    {
        // Act
        var user = new User { PasswordHash = passwordHash ?? string.Empty };

        // Assert
        user.PasswordHash.Should().Be(passwordHash ?? string.Empty);
    }
}

