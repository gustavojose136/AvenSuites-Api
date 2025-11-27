using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class InvoiceTests
{
    [Fact]
    public void Invoice_ShouldCreateWithValidData()
    {
        // Arrange
        var id = Guid.NewGuid();
        var bookingId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var totalServices = 500.00m;
        var totalTaxes = 50.00m;

        // Act
        var invoice = new Invoice
        {
            Id = id,
            BookingId = bookingId,
            HotelId = hotelId,
            Status = "PENDING",
            TotalServices = totalServices,
            TotalTaxes = totalTaxes
        };

        // Assert
        invoice.Id.Should().Be(id);
        invoice.BookingId.Should().Be(bookingId);
        invoice.HotelId.Should().Be(hotelId);
        invoice.Status.Should().Be("PENDING");
        invoice.TotalServices.Should().Be(totalServices);
        invoice.TotalTaxes.Should().Be(totalTaxes);
    }

    [Fact]
    public void Invoice_ShouldInitializeCollections()
    {
        // Act
        var invoice = new Invoice();

        // Assert
        invoice.Items.Should().NotBeNull();
        invoice.Items.Should().BeEmpty();
    }

    [Fact]
    public void Invoice_ShouldHaveDefaultValues()
    {
        // Act
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid()
        };

        // Assert
        invoice.Status.Should().Be("PENDING");
        invoice.TotalServices.Should().Be(0.00m);
        invoice.TotalTaxes.Should().Be(0.00m);
    }

    [Fact]
    public void Invoice_ShouldAllowOptionalProperties()
    {
        // Arrange
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid()
        };

        // Act
        invoice.NfseNumber = "123456";
        invoice.NfseSeries = "A";
        invoice.RpsNumber = "RPS-001";
        invoice.VerificationCode = "ABC123";
        invoice.IssueDate = DateTime.UtcNow;

        // Assert
        invoice.NfseNumber.Should().Be("123456");
        invoice.NfseSeries.Should().Be("A");
        invoice.RpsNumber.Should().Be("RPS-001");
        invoice.VerificationCode.Should().Be("ABC123");
        invoice.IssueDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

