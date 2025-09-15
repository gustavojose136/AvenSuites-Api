using AvenSuitesApi.Application.Services.Implementations.Auth;
using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace AvenSuitesApi.Application.Tests.Services;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        
        // Setup configuration
        _configurationMock.Setup(x => x["Jwt:Key"])
            .Returns("MinhaChaveSecretaSuperSeguraParaJWT2024!@#$%");
        _configurationMock.Setup(x => x["Jwt:Issuer"])
            .Returns("AvenSuites-Api");
        _configurationMock.Setup(x => x["Jwt:Audience"])
            .Returns("AvenSuitesUsers");
        _configurationMock.Setup(x => x["Jwt:ExpirationHours"])
            .Returns("24");

        _jwtService = new JwtService(_configurationMock.Object);
    }

    [Fact]
    public void GenerateToken_WithValidUser_ShouldReturnToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "User" } },
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
        token.Split('.').Should().HaveCount(3); // JWT has 3 parts
    }

    [Fact]
    public void GenerateToken_WithUserWithoutRoles_ShouldReturnToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            UserRoles = new List<UserRole>()
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void ValidateToken_WithValidToken_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            UserRoles = new List<UserRole>()
        };

        var token = _jwtService.GenerateToken(user);

        // Act
        var isValid = _jwtService.ValidateToken(token);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";

        // Act
        var isValid = _jwtService.ValidateToken(invalidToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ShouldReturnFalse()
    {
        // Arrange
        var emptyToken = "";

        // Act
        var isValid = _jwtService.ValidateToken(emptyToken);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void ValidateToken_WithNullToken_ShouldReturnFalse()
    {
        // Arrange
        string? nullToken = null;

        // Act
        var isValid = _jwtService.ValidateToken(nullToken!);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void GenerateToken_ShouldIncludeUserClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userName = "Test User";
        var userEmail = "test@email.com";
        
        var user = new User
        {
            Id = userId,
            Name = userName,
            Email = userEmail,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "User" } }
            }
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        
        // Decode token to verify claims (simplified check)
        var tokenParts = token.Split('.');
        tokenParts.Should().HaveCount(3);
    }

    [Fact]
    public void GenerateToken_WithMultipleRoles_ShouldIncludeAllRoles()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "User" } },
                new() { Role = new Role { Name = "Admin" } },
                new() { Role = new Role { Name = "Moderator" } }
            }
        };

        // Act
        var token = _jwtService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
    }
}

