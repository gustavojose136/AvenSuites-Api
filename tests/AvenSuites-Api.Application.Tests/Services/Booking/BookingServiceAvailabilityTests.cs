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

public class BookingServiceAvailabilityTests
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

    public BookingServiceAvailabilityTests()
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
    public async Task CreateBookingAsync_WithRoomConflict_ShouldReturnNull()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(3);

        var request = new BookingCreateRequest
        {
            HotelId = hotelId,
            Code = "RES-001",
            Source = "WEB",
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            Adults = 2,
            Children = 0,
            Currency = "BRL",
            MainGuestId = guestId,
            BookingRooms = new List<BookingRoomRequest>
            {
                new() { RoomId = roomId, RoomTypeId = Guid.NewGuid(), PriceTotal = 0 }
            }
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel { Id = hotelId, Name = "Hotel Test" };
        var guest = new AvenSuitesApi.Domain.Entities.Guest { Id = guestId, HotelId = hotelId };
        var room = new AvenSuitesApi.Domain.Entities.Room { Id = roomId, HotelId = hotelId, Status = "ACTIVE" };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _guestRepositoryMock.Setup(x => x.GetByUserId(guestId)).ReturnsAsync(guest);
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(roomId)).ReturnsAsync(room);
        _bookingRoomNightRepositoryMock.Setup(x => x.HasConflictAsync(roomId, checkIn, checkOut, null)).ReturnsAsync(true);

        // Act
        var result = await _bookingService.CreateBookingAsync(request);

        // Assert
        result.Should().BeNull();
        _bookingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>()), Times.Never);
    }

    [Fact]
    public async Task CreateBookingAsync_WithInactiveRoom_ShouldReturnNull()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        var request = new BookingCreateRequest
        {
            HotelId = hotelId,
            Code = "RES-001",
            Source = "WEB",
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            Adults = 2,
            Children = 0,
            Currency = "BRL",
            MainGuestId = guestId,
            BookingRooms = new List<BookingRoomRequest>
            {
                new() { RoomId = roomId, RoomTypeId = Guid.NewGuid(), PriceTotal = 0 }
            }
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel { Id = hotelId, Name = "Hotel Test" };
        var guest = new AvenSuitesApi.Domain.Entities.Guest { Id = guestId, HotelId = hotelId };
        var room = new AvenSuitesApi.Domain.Entities.Room { Id = roomId, HotelId = hotelId, Status = "INACTIVE" };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _guestRepositoryMock.Setup(x => x.GetByUserId(guestId)).ReturnsAsync(guest);
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(roomId)).ReturnsAsync(room);

        // Act
        var result = await _bookingService.CreateBookingAsync(request);

        // Assert
        result.Should().BeNull();
    }
}

