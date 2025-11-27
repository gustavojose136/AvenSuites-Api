using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class RatePlanRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly RatePlanRepository _repository;

    public RatePlanRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new RatePlanRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingRatePlan_ShouldReturnRatePlan()
    {
        // Arrange
        var ratePlan = new RatePlan
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            Name = "Flexível",
            Currency = "BRL",
            Active = true
        };

        _context.RatePlans.Add(ratePlan);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(ratePlan.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(ratePlan.Id);
        result.Name.Should().Be("Flexível");
    }

    [Fact]
    public async Task GetByIdWithPricesAsync_ShouldReturnRatePlanWithPrices()
    {
        // Arrange
        var ratePlan = new RatePlan
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            Name = "Flexível",
            Currency = "BRL",
            Active = true
        };

        _context.RatePlans.Add(ratePlan);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdWithPricesAsync(ratePlan.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Prices.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByHotelIdAsync_ShouldReturnAllRatePlansForHotel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var otherHotelId = Guid.NewGuid();

        var ratePlans = new List<RatePlan>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, RoomTypeId = Guid.NewGuid(), Name = "Flexível", Currency = "BRL", Active = true },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, RoomTypeId = Guid.NewGuid(), Name = "Não Reembolsável", Currency = "BRL", Active = true },
            new() { Id = Guid.NewGuid(), HotelId = otherHotelId, RoomTypeId = Guid.NewGuid(), Name = "Flexível", Currency = "BRL", Active = true }
        };

        _context.RatePlans.AddRange(ratePlans);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByHotelIdAsync(hotelId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(rp => rp.HotelId.Should().Be(hotelId));
    }

    [Fact]
    public async Task GetActiveByHotelIdAsync_ShouldReturnOnlyActiveRatePlans()
    {
        // Arrange
        var hotelId = Guid.NewGuid();

        var ratePlans = new List<RatePlan>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, RoomTypeId = Guid.NewGuid(), Name = "Ativo", Currency = "BRL", Active = true },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, RoomTypeId = Guid.NewGuid(), Name = "Inativo", Currency = "BRL", Active = false }
        };

        _context.RatePlans.AddRange(ratePlans);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveByHotelIdAsync(hotelId);

        // Assert
        result.Should().HaveCount(1);
        result.Should().AllSatisfy(rp => rp.Active.Should().BeTrue());
    }

    [Fact]
    public async Task AddAsync_WithValidRatePlan_ShouldAddRatePlan()
    {
        // Arrange
        var ratePlan = new RatePlan
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            RoomTypeId = Guid.NewGuid(),
            Name = "Novo Plano",
            Currency = "BRL",
            Active = true
        };

        // Act
        var result = await _repository.AddAsync(ratePlan);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(ratePlan.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

