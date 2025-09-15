using AvenSuitesApi.Application.DTOs;
using AvenSuitesApi.Application.Services.Implementations.Auth;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Application.Utils;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AvenSuitesApi.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _jwtServiceMock.Object);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@email.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = Argon2PasswordHasher.HashPassword("password123"),
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "User" } }
            }
        };

        var token = "jwt_token_here";

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync(user);
        _jwtServiceMock.Setup(x => x.GenerateToken(user))
            .Returns(token);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Token.Should().Be(token);
        result.User.Email.Should().Be(user.Email);
        result.User.Name.Should().Be(user.Name);
        result.User.Roles.Should().Contain("User");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@email.com",
            Password = "password123"
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@email.com",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = Argon2PasswordHasher.HashPassword("correctpassword"),
            IsActive = true,
            UserRoles = new List<UserRole>()
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInactiveUser_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "test@email.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = Argon2PasswordHasher.HashPassword("password123"),
            IsActive = false,
            UserRoles = new List<UserRole>()
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(loginRequest.Email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(loginRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ShouldReturnUserDto()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = "New User",
            Email = "newuser@email.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        var userRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "User",
            IsActive = true
        };

        var createdUser = new User
        {
            Id = Guid.NewGuid(),
            Name = registerRequest.Name,
            Email = registerRequest.Email,
            PasswordHash = Argon2PasswordHasher.HashPassword(registerRequest.Password),
            IsActive = true,
            UserRoles = new List<UserRole>
            {
                new() { Role = userRole }
            }
        };

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(registerRequest.Email))
            .ReturnsAsync(false);
        _roleRepositoryMock.Setup(x => x.GetByNameAsync("User"))
            .ReturnsAsync(userRole);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(createdUser);

        // Act
        var result = await _authService.RegisterAsync(registerRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(registerRequest.Name);
        result.Email.Should().Be(registerRequest.Email);
        result.Roles.Should().Contain("User");
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldReturnNull()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Name = "New User",
            Email = "existing@email.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(registerRequest.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(registerRequest);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithValidCredentials_ShouldReturnTrue()
    {
        // Arrange
        var email = "test@email.com";
        var password = "password123";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            PasswordHash = Argon2PasswordHasher.HashPassword(password),
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ValidatePasswordAsync(email, password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ValidatePasswordAsync_WithInvalidCredentials_ShouldReturnFalse()
    {
        // Arrange
        var email = "test@email.com";
        var password = "wrongpassword";

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = email,
            PasswordHash = Argon2PasswordHasher.HashPassword("correctpassword"),
            IsActive = true
        };

        _userRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ValidatePasswordAsync(email, password);

        // Assert
        result.Should().BeFalse();
    }
}

