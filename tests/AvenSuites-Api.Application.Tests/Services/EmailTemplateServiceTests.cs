using AvenSuitesApi.Application.Services.Implementations;
using FluentAssertions;

namespace AvenSuitesApi.Application.Tests.Services;

public class EmailTemplateServiceTests
{
    private readonly EmailTemplateService _emailTemplateService;

    public EmailTemplateServiceTests()
    {
        _emailTemplateService = new EmailTemplateService();
    }

    [Fact]
    public void GenerateWelcomeEmail_ShouldContainGuestName()
    {
        // Arrange
        var guestName = "João Silva";
        var hotelName = "Hotel Teste";

        // Act
        var result = _emailTemplateService.GenerateWelcomeEmail(guestName, hotelName);

        // Assert
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain("<!DOCTYPE html>");
    }

    [Fact]
    public void GenerateBookingConfirmationEmail_ShouldContainBookingDetails()
    {
        // Arrange
        var guestName = "Maria Santos";
        var hotelName = "Hotel Teste";
        var bookingCode = "ABC123";
        var checkIn = DateTime.Now.AddDays(7);
        var checkOut = DateTime.Now.AddDays(10);
        var rooms = new List<AvenSuitesApi.Application.Services.Interfaces.BookingRoomInfo>
        {
            new() { RoomNumber = "101", RoomTypeName = "Standard", PriceTotal = 500m }
        };

        // Act
        var result = _emailTemplateService.GenerateBookingConfirmationEmail(
            guestName, hotelName, bookingCode, checkIn, checkOut, 3, 500m, "BRL", rooms);

        // Assert
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain(bookingCode);
        result.Should().Contain("101");
        result.Should().Contain("Standard");
    }

    [Fact]
    public void GenerateBookingCancellationEmail_ShouldContainCancellationInfo()
    {
        // Arrange
        var guestName = "Pedro Costa";
        var hotelName = "Hotel Teste";
        var bookingCode = "XYZ789";
        var checkIn = DateTime.Now.AddDays(5);
        var checkOut = DateTime.Now.AddDays(8);

        // Act
        var result = _emailTemplateService.GenerateBookingCancellationEmail(
            guestName, hotelName, bookingCode, checkIn, checkOut, 1000m, "BRL", "Motivo do cancelamento");

        // Assert
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain(bookingCode);
        result.Should().Contain("Motivo do cancelamento");
    }

    [Fact]
    public void GenerateBookingReminderEmail_ShouldContainReminderInfo()
    {
        // Arrange
        var guestName = "Ana Lima";
        var hotelName = "Hotel Teste";
        var bookingCode = "REM456";
        var checkIn = DateTime.Now.AddDays(3);
        var checkOut = DateTime.Now.AddDays(6);
        var rooms = new List<AvenSuitesApi.Application.Services.Interfaces.BookingRoomInfo>
        {
            new() { RoomNumber = "201", RoomTypeName = "Deluxe", PriceTotal = 800m }
        };

        // Act
        var result = _emailTemplateService.GenerateBookingReminderEmail(
            guestName, hotelName, bookingCode, checkIn, checkOut, 3, rooms);

        // Assert
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain(bookingCode);
        result.Should().Contain("201");
    }

    [Fact]
    public void GenerateCheckInConfirmationEmail_ShouldContainCheckInInfo()
    {
        // Arrange
        var guestName = "Carlos Oliveira";
        var hotelName = "Hotel Teste";
        var bookingCode = "CHK789";
        var checkOut = DateTime.Now.AddDays(5);
        var rooms = new List<AvenSuitesApi.Application.Services.Interfaces.BookingRoomInfo>
        {
            new() { RoomNumber = "301", RoomTypeName = "Suite", PriceTotal = 1200m }
        };

        // Act
        var result = _emailTemplateService.GenerateCheckInConfirmationEmail(
            guestName, hotelName, bookingCode, rooms, checkOut);

        // Assert
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain(bookingCode);
        result.Should().Contain("301");
    }

    [Fact]
    public void GenerateCheckOutConfirmationEmail_ShouldContainCheckOutInfo()
    {
        // Arrange
        var guestName = "Fernanda Souza";
        var hotelName = "Hotel Teste";
        var bookingCode = "OUT123";
        var checkIn = DateTime.Now.AddDays(-3);
        var checkOut = DateTime.Now;

        // Act
        var result = _emailTemplateService.GenerateCheckOutConfirmationEmail(
            guestName, hotelName, bookingCode, checkIn, checkOut, 3, 1500m, "BRL");

        // Assert
        result.Should().Contain(guestName);
        result.Should().Contain(hotelName);
        result.Should().Contain(bookingCode);
    }
}


