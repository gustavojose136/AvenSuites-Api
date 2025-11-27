using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class AmenityRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AmenityRepository _amenityRepository;

    public AmenityRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _amenityRepository = new AmenityRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingAmenity_ShouldReturnAmenity()
    {
        // Arrange
        var amenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Code = "WIFI",
            Name = "Wi-Fi"
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _amenityRepository.GetByIdAsync(amenity.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(amenity.Id);
        result.Code.Should().Be(amenity.Code);
        result.Name.Should().Be(amenity.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingAmenity_ShouldReturnNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _amenityRepository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodeAsync_WithExistingCode_ShouldReturnAmenity()
    {
        // Arrange
        var amenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Code = "AC",
            Name = "Ar Condicionado"
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _amenityRepository.GetByCodeAsync("AC");

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("AC");
    }

    [Fact]
    public async Task GetByCodeAsync_WithNonExistingCode_ShouldReturnNull()
    {
        // Arrange & Act
        var result = await _amenityRepository.GetByCodeAsync("NONEXISTENT");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllAmenities()
    {
        // Arrange
        var amenities = new List<Amenity>
        {
            new() { Id = Guid.NewGuid(), Code = "WIFI", Name = "Wi-Fi" },
            new() { Id = Guid.NewGuid(), Code = "AC", Name = "Ar Condicionado" },
            new() { Id = Guid.NewGuid(), Code = "TV", Name = "TV" }
        };

        _context.Amenities.AddRange(amenities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _amenityRepository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(a => a.Code == "WIFI");
        result.Should().Contain(a => a.Code == "AC");
        result.Should().Contain(a => a.Code == "TV");
    }

    [Fact]
    public async Task AddAsync_WithValidAmenity_ShouldAddAmenity()
    {
        // Arrange
        var amenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Code = "POOL",
            Name = "Piscina"
        };

        // Act
        var result = await _amenityRepository.AddAsync(amenity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(amenity.Id);
        result.Code.Should().Be("POOL");

        var savedAmenity = await _context.Amenities.FindAsync(amenity.Id);
        savedAmenity.Should().NotBeNull();
        savedAmenity!.Code.Should().Be("POOL");
    }

    [Fact]
    public async Task UpdateAsync_WithExistingAmenity_ShouldUpdateAmenity()
    {
        // Arrange
        var amenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Code = "WIFI",
            Name = "Wi-Fi Original"
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        amenity.Name = "Wi-Fi Atualizado";

        // Act
        var result = await _amenityRepository.UpdateAsync(amenity);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Wi-Fi Atualizado");

        var updatedAmenity = await _context.Amenities.FindAsync(amenity.Id);
        updatedAmenity!.Name.Should().Be("Wi-Fi Atualizado");
    }

    [Fact]
    public async Task DeleteAsync_WithExistingAmenity_ShouldDeleteAmenity()
    {
        // Arrange
        var amenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Code = "TV",
            Name = "TV"
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        // Act
        await _amenityRepository.DeleteAsync(amenity.Id);

        // Assert
        var deletedAmenity = await _context.Amenities.FindAsync(amenity.Id);
        deletedAmenity.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingAmenity_ShouldNotThrow()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var act = async () => await _amenityRepository.DeleteAsync(nonExistingId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ExistsByCodeAsync_WithExistingCode_ShouldReturnTrue()
    {
        // Arrange
        var amenity = new Amenity
        {
            Id = Guid.NewGuid(),
            Code = "WIFI",
            Name = "Wi-Fi"
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _amenityRepository.ExistsByCodeAsync("WIFI");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByCodeAsync_WithNonExistingCode_ShouldReturnFalse()
    {
        // Arrange & Act
        var exists = await _amenityRepository.ExistsByCodeAsync("NONEXISTENT");

        // Assert
        exists.Should().BeFalse();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

