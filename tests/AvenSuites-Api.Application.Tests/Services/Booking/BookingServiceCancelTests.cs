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

public class BookingServiceCancelTests
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

    public BookingServiceCancelTests()
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
    public async Task CancelBookingAsync_WithNonExistingBooking_ShouldReturnFalse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.CancelBookingAsync(bookingId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CancelBookingAsync_WithExistingBooking_ShouldCancelAndDeleteNights()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var bookingRoomId1 = Guid.NewGuid();
        var bookingRoomId2 = Guid.NewGuid();
        
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel Test",
            Email = "hotel@test.com"
        };

        var guestPii = new GuestPii
        {
            GuestId = Guid.NewGuid(),
            FullName = "João Silva",
            Email = "joao@test.com"
        };

        var guest = new Guest
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            GuestPii = guestPii
        };

        var booking = new Booking
        {
            Id = bookingId,
            HotelId = hotel.Id,
            Code = "RES-001",
            Status = "CONFIRMED",
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            MainGuestId = guest.Id,
            Currency = "BRL",
            TotalAmount = 600m,
            Hotel = hotel,
            MainGuest = guest,
            BookingRooms = new List<BookingRoom>
            {
                new() { Id = bookingRoomId1, RoomId = Guid.NewGuid() },
                new() { Id = bookingRoomId2, RoomId = Guid.NewGuid() }
            }
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Booking>())).ReturnsAsync(booking);
        _bookingRoomNightRepositoryMock.Setup(x => x.DeleteByBookingRoomIdAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        // Act
        var result = await _bookingService.CancelBookingAsync(bookingId, "Cancelamento de teste");

        // Assert
        result.Should().BeTrue();
        _bookingRoomNightRepositoryMock.Verify(x => x.DeleteByBookingRoomIdAsync(bookingRoomId1), Times.Once);
        _bookingRoomNightRepositoryMock.Verify(x => x.DeleteByBookingRoomIdAsync(bookingRoomId2), Times.Once);
        _bookingRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Booking>(b => b.Status == "CANCELLED")), Times.Once);
    }

    [Fact]
    public async Task ConfirmBookingAsync_WithValidBooking_ShouldUpdateStatus()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new Booking
        {
            Id = bookingId,
            Status = "PENDING",
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            CheckInDate = DateTime.Today,
            CheckOutDate = DateTime.Today.AddDays(2),
            MainGuestId = Guid.NewGuid(),
            Currency = "BRL"
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Booking>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.ConfirmBookingAsync(bookingId);

        // Assert
        result.Should().BeTrue();
        _bookingRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Booking>(b => b.Status == "CONFIRMED")), Times.Once);
    }

    [Fact]
    public async Task ConfirmBookingAsync_WithNonExistingBooking_ShouldReturnFalse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _bookingService.ConfirmBookingAsync(bookingId);

        // Assert
        result.Should().BeFalse();
    }
}

