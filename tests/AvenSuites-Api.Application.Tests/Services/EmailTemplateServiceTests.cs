using AvenSuitesApi.Application.Services.Implementations;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Application.Tests.Services;

public class EmailTemplateServiceTests
{
    private readonly EmailTemplateService _service;

    public EmailTemplateServiceTests()
    {
        _service = new EmailTemplateService();
    }

    [Fact]
    public void GenerateWelcomeEmail_WithValidData_ShouldReturnHtmlContent()
    {
        // Arrange
        var guestName = "João Silva";
        var hotelName = "Hotel Avenida";

        // Act
        var result = _service.GenerateWelcomeEmail(guestName, hotelName);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain("<!DOCTYPE html>");
        result.Should().Contain("</html>");
    }

    [Fact]
    public void GenerateBookingConfirmationEmail_WithValidData_ShouldReturnHtmlContent()
    {
        // Arrange
        var guestName = "Maria Santos";
        var hotelName = "Hotel Avenida";
        var bookingCode = "RES-2025-001";
        var checkInDate = DateTime.Today.AddDays(7);
        var checkOutDate = DateTime.Today.AddDays(9);
        var totalAmount = 600m;
        var currency = "BRL";

        // Act
        var result = _service.GenerateBookingConfirmationEmail(
            guestName, hotelName, bookingCode, checkInDate, checkOutDate, totalAmount, currency);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain(bookingCode);
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GenerateBookingCancellationEmail_WithValidData_ShouldReturnHtmlContent()
    {
        // Arrange
        var guestName = "Pedro Costa";
        var hotelName = "Hotel Avenida";
        var bookingCode = "RES-2025-002";
        var checkInDate = DateTime.Today.AddDays(5);
        var checkOutDate = DateTime.Today.AddDays(7);
        var totalAmount = 400m;
        var currency = "BRL";
        var reason = "Cancelamento solicitado pelo cliente";

        // Act
        var result = _service.GenerateBookingCancellationEmail(
            guestName, hotelName, bookingCode, checkInDate, checkOutDate, totalAmount, currency, reason);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(guestName);
        result.Should().Contain(bookingCode);
        result.Should().Contain(reason);
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GenerateInvoiceEmail_WithValidData_ShouldReturnHtmlContent()
    {
        // Arrange
        var guestName = "Ana Lima";
        var hotelName = "Hotel Avenida";
        var invoiceNumber = "NF-2025-001";
        var invoiceDate = DateTime.Today;
        var totalAmount = 500m;
        var currency = "BRL";

        // Act
        var result = _service.GenerateInvoiceEmail(
            guestName, hotelName, invoiceNumber, invoiceDate, totalAmount, currency);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain(guestName);
        result.Should().Contain(invoiceNumber);
        result.Should().Contain("<!DOCTYPE html>");
    }
}

