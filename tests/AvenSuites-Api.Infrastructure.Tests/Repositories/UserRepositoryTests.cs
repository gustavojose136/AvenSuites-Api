using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _userRepository = new UserRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingUser_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _userRepository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByEmailAsync_WithExistingUser_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetByEmailAsync(user.Email);

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be(user.Email);
        result.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
    {
        // Arrange
        var nonExistingEmail = "nonexistent@email.com";

        // Act
        var result = await _userRepository.GetByEmailAsync(nonExistingEmail);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveUsers()
    {
        // Arrange
        var activeUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Active User",
            Email = "active@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var inactiveUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Inactive User",
            Email = "inactive@email.com",
            PasswordHash = "hashed_password",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.AddRange(activeUser, inactiveUser);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(u => u.Id == activeUser.Id);
        result.Should().NotContain(u => u.Id == inactiveUser.Id);
    }

    [Fact]
    public async Task AddAsync_WithValidUser_ShouldAddUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "New User",
            Email = "newuser@email.com",
            PasswordHash = "hashed_password",
            IsActive = true
        };

        // Act
        var result = await _userRepository.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
        result.Email.Should().Be(user.Email);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedUser = await _context.Users.FindAsync(user.Id);
        savedUser.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingUser_ShouldUpdateUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Original Name",
            Email = "original@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        user.Name = "Updated Name";
        user.Email = "updated@email.com";

        // Act
        var result = await _userRepository.UpdateAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        result.Email.Should().Be("updated@email.com");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedUser = await _context.Users.FindAsync(user.Id);
        savedUser!.Name.Should().Be("Updated Name");
        savedUser.Email.Should().Be("updated@email.com");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ShouldSoftDeleteUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "User to Delete",
            Email = "delete@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _userRepository.DeleteAsync(user.Id);

        // Assert
        var deletedUser = await _context.Users.FindAsync(user.Id);
        deletedUser.Should().NotBeNull();
        deletedUser!.IsActive.Should().BeFalse();
        deletedUser.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExistsAsync_WithExistingUser_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = "existing@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _userRepository.ExistsAsync(user.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var exists = await _userRepository.ExistsAsync(nonExistingId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithExistingEmail_ShouldReturnTrue()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = "existing@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _userRepository.ExistsByEmailAsync(user.Email);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByEmailAsync_WithNonExistingEmail_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingEmail = "nonexistent@email.com";

        // Act
        var exists = await _userRepository.ExistsByEmailAsync(nonExistingEmail);

        // Assert
        exists.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

