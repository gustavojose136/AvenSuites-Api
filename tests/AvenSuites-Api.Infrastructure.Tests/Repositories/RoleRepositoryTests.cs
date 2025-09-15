using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class RoleRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RoleRepository _roleRepository;

    public RoleRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _roleRepository = new RoleRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingRole_ShouldReturnRole()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            Description = "Administrator role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleRepository.GetByIdAsync(role.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(role.Id);
        result.Name.Should().Be(role.Name);
        result.Description.Should().Be(role.Description);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingRole_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _roleRepository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByNameAsync_WithExistingRole_ShouldReturnRole()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "User",
            Description = "Regular user role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleRepository.GetByNameAsync(role.Name);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(role.Name);
        result.Description.Should().Be(role.Description);
    }

    [Fact]
    public async Task GetByNameAsync_WithNonExistingRole_ShouldReturnNull()
    {
        // Arrange
        var nonExistingName = "NonExistingRole";

        // Act
        var result = await _roleRepository.GetByNameAsync(nonExistingName);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveRoles()
    {
        // Arrange
        var activeRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "ActiveRole",
            Description = "Active role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var inactiveRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "InactiveRole",
            Description = "Inactive role",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.AddRange(activeRole, inactiveRole);
        await _context.SaveChangesAsync();

        // Act
        var result = await _roleRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(r => r.Id == activeRole.Id);
        result.Should().NotContain(r => r.Id == inactiveRole.Id);
    }

    [Fact]
    public async Task AddAsync_WithValidRole_ShouldAddRole()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "NewRole",
            Description = "New role description",
            IsActive = true
        };

        // Act
        var result = await _roleRepository.AddAsync(role);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(role.Id);
        result.Name.Should().Be(role.Name);
        result.Description.Should().Be(role.Description);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedRole = await _context.Roles.FindAsync(role.Id);
        savedRole.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingRole_ShouldUpdateRole()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "OriginalRole",
            Description = "Original description",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        role.Name = "UpdatedRole";
        role.Description = "Updated description";

        // Act
        var result = await _roleRepository.UpdateAsync(role);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("UpdatedRole");
        result.Description.Should().Be("Updated description");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedRole = await _context.Roles.FindAsync(role.Id);
        savedRole!.Name.Should().Be("UpdatedRole");
        savedRole.Description.Should().Be("Updated description");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingRole_ShouldSoftDeleteRole()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "RoleToDelete",
            Description = "Role to be deleted",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        await _roleRepository.DeleteAsync(role.Id);

        // Assert
        var deletedRole = await _context.Roles.FindAsync(role.Id);
        deletedRole.Should().NotBeNull();
        deletedRole!.IsActive.Should().BeFalse();
        deletedRole.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task ExistsAsync_WithExistingRole_ShouldReturnTrue()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "ExistingRole",
            Description = "Existing role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _roleRepository.ExistsAsync(role.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingRole_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var exists = await _roleRepository.ExistsAsync(nonExistingId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByNameAsync_WithExistingName_ShouldReturnTrue()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "ExistingRole",
            Description = "Existing role",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Roles.Add(role);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _roleRepository.ExistsByNameAsync(role.Name);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_WithNonExistingName_ShouldReturnFalse()
    {
        // Arrange
        var nonExistingName = "NonExistingRole";

        // Act
        var exists = await _roleRepository.ExistsByNameAsync(nonExistingName);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_WithNullDescription_ShouldWork()
    {
        // Arrange
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "RoleWithoutDescription",
            Description = null,
            IsActive = true
        };

        // Act
        var result = await _roleRepository.AddAsync(role);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

