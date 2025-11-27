using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Booking;
using AvenSuitesApi.Application.Services.Implementations.Booking;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using RoomType = AvenSuitesApi.Domain.Entities.RoomType;
using Xunit;

namespace AvenSuites_Api.Application.Tests.Services.Booking;

public class BookingServiceTests
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

    public BookingServiceTests()
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
    public async Task CreateBookingAsync_WithValidRequest_ShouldReturnBookingResponse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var roomTypeId = Guid.NewGuid();

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
            ChannelRef = "WEB-001",
            BookingRooms = new List<BookingRoomRequest>
            {
                new()
                {
                    RoomId = roomId,
                    RoomTypeId = roomTypeId,
                    PriceTotal = 300.00m,
                    RatePlanId = null
                }
            }
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = hotelId,
            Name = "Hotel Test",
            Status = "ACTIVE"
        };

        var guest = new AvenSuitesApi.Domain.Entities.Guest
        {
            Id = guestId,
            HotelId = hotelId,
            MarketingConsent = true
        };

        var room = new AvenSuitesApi.Domain.Entities.Room
        {
            Id = roomId,
            HotelId = hotelId,
            RoomTypeId = roomTypeId,
            RoomNumber = "101",
            Status = "ACTIVE"
        };

        var bookingRoom = new BookingRoom
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            RoomTypeId = roomTypeId,
            PriceTotal = 300.00m,
            Room = new AvenSuitesApi.Domain.Entities.Room
            {
                Id = roomId,
                RoomNumber = "101"
            },
            RoomType = new RoomType
            {
                Id = roomTypeId,
                Name = "Standard"
            }
        };

        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Code = request.Code,
            Status = "PENDING",
            Source = request.Source,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Adults = request.Adults,
            Children = request.Children,
            Currency = request.Currency,
            MainGuestId = guestId,
            TotalAmount = 300.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookingRooms = new List<BookingRoom> { bookingRoom },
            Payments = new List<BookingPayment>()
        };

        var roomType = new RoomType
        {
            Id = roomTypeId,
            BasePrice = 150.00m,
            OccupancyPrices = new List<RoomTypeOccupancyPrice>
            {
                new() { Occupancy = 2, PricePerNight = 150.00m }
            }
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _guestRepositoryMock.Setup(x => x.GetByUserId(guestId)).ReturnsAsync(guest);
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(roomId)).ReturnsAsync(room);
        _bookingRoomNightRepositoryMock.Setup(x => x.HasConflictAsync(roomId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), null)).ReturnsAsync(false);
        _roomTypeRepositoryMock.Setup(x => x.GetByIdWithOccupancyPricesAsync(roomTypeId)).ReturnsAsync(roomType);
        _bookingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);
        _bookingRoomRepositoryMock.Setup(x => x.AddAsync(It.IsAny<BookingRoom>())).ReturnsAsync(new BookingRoom { Id = Guid.NewGuid() });
        _bookingRoomNightRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<BookingRoomNight>>())).Returns(Task.CompletedTask);
        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CreateBookingAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(request.Code);
        result.Status.Should().Be("PENDING");
        result.TotalAmount.Should().BeGreaterThan(0);

        _bookingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookingAsync_WithInvalidHotel_ShouldReturnNull()
    {
        // Arrange
        var request = new BookingCreateRequest
        {
            HotelId = Guid.NewGuid(),
            Code = "RES-001"
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((AvenSuitesApi.Domain.Entities.Hotel?)null);

        // Act
        var result = await _bookingService.CreateBookingAsync(request);

        // Assert
        result.Should().BeNull();
        _bookingRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>()), Times.Never);
    }

    [Fact]
    public async Task GetBookingByIdAsync_WithValidId_ShouldReturnBooking()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = bookingId,
            Code = "RES-001",
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookingRooms = new List<BookingRoom>(),
            Payments = new List<BookingPayment>()
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.GetBookingByIdAsync(bookingId);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("RES-001");
    }

    [Fact]
    public async Task CancelBookingAsync_WithValidBooking_ShouldReturnTrue()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = bookingId,
            Status = "CONFIRMED",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookingRooms = new List<BookingRoom>(),
            Payments = new List<BookingPayment>()
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);
        _bookingRoomNightRepositoryMock.Setup(x => x.DeleteByBookingRoomIdAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);

        // Act
        var result = await _bookingService.CancelBookingAsync(bookingId, "Cliente cancelou");

        // Assert
        result.Should().BeTrue();
        _bookingRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmBookingAsync_WithValidBooking_ShouldReturnTrue()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = bookingId,
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookingRooms = new List<BookingRoom>(),
            Payments = new List<BookingPayment>()
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.ConfirmBookingAsync(bookingId);

        // Assert
        result.Should().BeTrue();
        _bookingRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>()), Times.Once);
    }
}

