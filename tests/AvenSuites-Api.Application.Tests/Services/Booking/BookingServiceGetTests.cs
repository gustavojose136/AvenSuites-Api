using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Booking;
using AvenSuitesApi.Application.Services.Implementations.Booking;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AvenSuitesApi.Application.Tests.Services.Booking;

public class BookingServiceGetTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IBookingRoomRepository> _bookingRoomRepositoryMock;
    private readonly Mock<IBookingRoomNightRepository> _bookingRoomNightRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IGuestRepository> _guestRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IRoomTypeRepository> _roomTypeRepositoryMock;
    private readonly Mock<IRatePlanRepository> _ratePlanRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<ILogger<BookingService>> _loggerMock;
    private readonly BookingService _bookingService;

    public BookingServiceGetTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _bookingRoomRepositoryMock = new Mock<IBookingRoomRepository>();
        _bookingRoomNightRepositoryMock = new Mock<IBookingRoomNightRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _guestRepositoryMock = new Mock<IGuestRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _roomTypeRepositoryMock = new Mock<IRoomTypeRepository>();
        _ratePlanRepositoryMock = new Mock<IRatePlanRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<BookingService>>();

        _bookingService = new BookingService(
            _bookingRepositoryMock.Object,
            _bookingRoomRepositoryMock.Object,
            _bookingRoomNightRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _guestRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _roomTypeRepositoryMock.Object,
            _ratePlanRepositoryMock.Object,
            _emailServiceMock.Object,
            _emailTemplateServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            Status = "CONFIRMED",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL",
            TotalAmount = 400m
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("RES-001");
        result.Status.Should().Be("CONFIRMED");
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBookingByCodeAsync_WithValidCode_ShouldReturnBooking()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var code = "RES-001";
        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Code = code,
            Status = "PENDING",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL"
        };

        _bookingRepositoryMock.Setup(x => x.GetByCodeAsync(hotelId, code)).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetBookingByCodeAsync(hotelId, code);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(code);
    }

    [Fact]
    public async Task GetBookingsByHotelAsync_WithDateRange_ShouldReturnFilteredBookings()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);

        var bookings = new List<Booking>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, Code = "RES-001", Status = "CONFIRMED", CheckInDate = DateTime.Today.AddDays(1), CheckOutDate = DateTime.Today.AddDays(3), MainGuestId = Guid.NewGuid(), Currency = "BRL" },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, Code = "RES-002", Status = "PENDING", CheckInDate = DateTime.Today.AddDays(5), CheckOutDate = DateTime.Today.AddDays(7), MainGuestId = Guid.NewGuid(), Currency = "BRL" }
        };

        _bookingRepositoryMock.Setup(x => x.GetByHotelIdAndDatesAsync(hotelId, startDate, endDate)).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByHotelAsync(hotelId, startDate, endDate);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBookingsByHotelAsync_WithoutDateRange_ShouldReturnAllBookings()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, Code = "RES-001", Status = "CONFIRMED", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(2), MainGuestId = Guid.NewGuid(), Currency = "BRL" }
        };

        _bookingRepositoryMock.Setup(x => x.GetByHotelIdAsync(hotelId)).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByHotelAsync(hotelId);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetBookingsByGuestAsync_WithValidGuestId_ShouldReturnBookings()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        var bookings = new List<Booking>
        {
            new() { Id = Guid.NewGuid(), HotelId = Guid.NewGuid(), Code = "RES-001", Status = "CONFIRMED", CheckInDate = DateTime.Today, CheckOutDate = DateTime.Today.AddDays(2), MainGuestId = guestId, Currency = "BRL" }
        };

        _bookingRepositoryMock.Setup(x => x.GetByGuestIdAsync(guestId)).ReturnsAsync(bookings);

        // Act
        var result = await _bookingService.GetBookingsByGuestAsync(guestId);

        // Assert
        result.Should().HaveCount(1);
    }
}

