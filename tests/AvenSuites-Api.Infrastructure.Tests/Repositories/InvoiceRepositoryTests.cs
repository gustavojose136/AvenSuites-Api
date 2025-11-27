using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AvenSuitesApi.Infrastructure.Tests.Repositories;

public class InvoiceRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly InvoiceRepository _repository;

    public InvoiceRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new InvoiceRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingInvoice_ShouldReturnInvoice()
    {
        // Arrange
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Status = "PENDING",
            TotalServices = 500m,
            TotalTaxes = 50m
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(invoice.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(invoice.Id);
        result.Status.Should().Be("PENDING");
    }

    [Fact]
    public async Task GetByBookingIdAsync_WithExistingInvoice_ShouldReturnInvoice()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            HotelId = Guid.NewGuid(),
            Status = "PENDING",
            TotalServices = 500m
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByBookingIdAsync(bookingId);

        // Assert
        result.Should().NotBeNull();
        result!.BookingId.Should().Be(bookingId);
    }

    [Fact]
    public async Task GetByHotelIdAsync_ShouldReturnAllInvoicesForHotel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var otherHotelId = Guid.NewGuid();

        var invoices = new List<Invoice>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, BookingId = Guid.NewGuid(), Status = "PENDING", TotalServices = 100m },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, BookingId = Guid.NewGuid(), Status = "ISSUED", TotalServices = 200m },
            new() { Id = Guid.NewGuid(), HotelId = otherHotelId, BookingId = Guid.NewGuid(), Status = "PENDING", TotalServices = 300m }
        };

        _context.Invoices.AddRange(invoices);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByHotelIdAsync(hotelId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(i => i.HotelId.Should().Be(hotelId));
    }

    [Fact]
    public async Task AddAsync_WithValidInvoice_ShouldAddInvoice()
    {
        // Arrange
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Status = "PENDING",
            TotalServices = 500m,
            TotalTaxes = 50m
        };

        // Act
        var result = await _repository.AddAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(invoice.Id);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var savedInvoice = await _context.Invoices.FindAsync(invoice.Id);
        savedInvoice.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WithExistingInvoice_ShouldUpdateInvoice()
    {
        // Arrange
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Status = "PENDING",
            TotalServices = 500m
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        invoice.Status = "ISSUED";
        invoice.IssueDate = DateTime.UtcNow;

        // Act
        var result = await _repository.UpdateAsync(invoice);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be("ISSUED");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

