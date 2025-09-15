using AvenSuitesApi.Domain.Entities;
using FluentAssertions;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class UserRoleTests
{
    [Fact]
    public void UserRole_ShouldCreateWithValidData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow;

        // Act
        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = roleId,
            AssignedAt = assignedAt
        };

        // Assert
        userRole.UserId.Should().Be(userId);
        userRole.RoleId.Should().Be(roleId);
        userRole.AssignedAt.Should().Be(assignedAt);
        userRole.User.Should().BeNull();
        userRole.Role.Should().BeNull();
    }

    [Fact]
    public void UserRole_ShouldAllowSettingUser()
    {
        // Arrange
        var userRole = new UserRole();
        var user = new User { Id = Guid.NewGuid(), Name = "Test User" };

        // Act
        userRole.User = user;

        // Assert
        userRole.User.Should().Be(user);
    }

    [Fact]
    public void UserRole_ShouldAllowSettingRole()
    {
        // Arrange
        var userRole = new UserRole();
        var role = new Role { Id = Guid.NewGuid(), Name = "Test Role" };

        // Act
        userRole.Role = role;

        // Assert
        userRole.Role.Should().Be(role);
    }

    [Fact]
    public void UserRole_ShouldAllowSettingAssignedAt()
    {
        // Arrange
        var userRole = new UserRole();
        var assignedAt = DateTime.UtcNow.AddDays(-1);

        // Act
        userRole.AssignedAt = assignedAt;

        // Assert
        userRole.AssignedAt.Should().Be(assignedAt);
    }

    [Fact]
    public void UserRole_ShouldInitializeWithDefaultValues()
    {
        // Act
        var userRole = new UserRole();

        // Assert
        userRole.UserId.Should().Be(Guid.Empty);
        userRole.RoleId.Should().Be(Guid.Empty);
        userRole.AssignedAt.Should().Be(default(DateTime));
        userRole.User.Should().BeNull();
        userRole.Role.Should().BeNull();
    }
}

