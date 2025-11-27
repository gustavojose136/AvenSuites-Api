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

public class BookingServiceOccupancyTests
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

    public BookingServiceOccupancyTests()
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
    public async Task CreateBookingAsync_WithOccupancyPrice_ShouldUseOccupancyPrice()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var roomTypeId = Guid.NewGuid();
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(3);
        var nights = (checkOut - checkIn).Days;

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
                new() { RoomId = roomId, RoomTypeId = roomTypeId, PriceTotal = 0 }
            }
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel { Id = hotelId, Name = "Hotel Test" };
        var guest = new AvenSuitesApi.Domain.Entities.Guest { Id = guestId, HotelId = hotelId };
        var room = new AvenSuitesApi.Domain.Entities.Room { Id = roomId, HotelId = hotelId, Status = "ACTIVE" };
        var roomType = new RoomType
        {
            Id = roomTypeId,
            BasePrice = 100.00m,
            OccupancyPrices = new List<RoomTypeOccupancyPrice>
            {
                new() { Occupancy = 2, PricePerNight = 150.00m }
            }
        };

        var bookingRoom = new BookingRoom
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            RoomTypeId = roomTypeId,
            PriceTotal = 300.00m,
            Room = new AvenSuitesApi.Domain.Entities.Room { Id = roomId, RoomNumber = "101" },
            RoomType = new RoomType { Id = roomTypeId, Name = "Standard" }
        };

        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Code = request.Code,
            Status = "PENDING",
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            Adults = 2,
            Children = 0,
            TotalAmount = 300.00m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookingRooms = new List<BookingRoom> { bookingRoom },
            Payments = new List<BookingPayment>()
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _guestRepositoryMock.Setup(x => x.GetByUserId(guestId)).ReturnsAsync(guest);
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(roomId)).ReturnsAsync(room);
        _bookingRoomNightRepositoryMock.Setup(x => x.HasConflictAsync(roomId, checkIn, checkOut, null)).ReturnsAsync(false);
        _roomTypeRepositoryMock.Setup(x => x.GetByIdWithOccupancyPricesAsync(roomTypeId)).ReturnsAsync(roomType);
        _bookingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);
        _bookingRoomRepositoryMock.Setup(x => x.AddAsync(It.IsAny<BookingRoom>())).ReturnsAsync(new BookingRoom { Id = Guid.NewGuid() });
        _bookingRoomNightRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<BookingRoomNight>>())).Returns(Task.CompletedTask);
        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CreateBookingAsync(request);

        // Assert
        result.Should().NotBeNull();
        // O preço deve ser 150.00 (ocupação 2) × 2 noites = 300.00
        result!.TotalAmount.Should().Be(300.00m);
    }

    [Fact]
    public async Task CreateBookingAsync_WithoutOccupancyPrice_ShouldUseBasePrice()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        var roomTypeId = Guid.NewGuid();
        var checkIn = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(3);
        var nights = (checkOut - checkIn).Days;

        var request = new BookingCreateRequest
        {
            HotelId = hotelId,
            Code = "RES-001",
            Source = "WEB",
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            Adults = 5, // Ocupação sem preço específico
            Children = 0,
            Currency = "BRL",
            MainGuestId = guestId,
            BookingRooms = new List<BookingRoomRequest>
            {
                new() { RoomId = roomId, RoomTypeId = roomTypeId, PriceTotal = 0 }
            }
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel { Id = hotelId, Name = "Hotel Test" };
        var guest = new AvenSuitesApi.Domain.Entities.Guest { Id = guestId, HotelId = hotelId };
        var room = new AvenSuitesApi.Domain.Entities.Room { Id = roomId, HotelId = hotelId, Status = "ACTIVE" };
        var roomType = new RoomType
        {
            Id = roomTypeId,
            BasePrice = 200.00m,
            OccupancyPrices = new List<RoomTypeOccupancyPrice>
            {
                new() { Occupancy = 2, PricePerNight = 150.00m }
            }
        };

        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Code = request.Code,
            Status = "PENDING",
            CheckInDate = checkIn,
            CheckOutDate = checkOut,
            Adults = 5,
            Children = 0,
            TotalAmount = 0m
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _guestRepositoryMock.Setup(x => x.GetByUserId(guestId)).ReturnsAsync(guest);
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(roomId)).ReturnsAsync(room);
        _bookingRoomNightRepositoryMock.Setup(x => x.HasConflictAsync(roomId, checkIn, checkOut, null)).ReturnsAsync(false);
        _roomTypeRepositoryMock.Setup(x => x.GetByIdWithOccupancyPricesAsync(roomTypeId)).ReturnsAsync(roomType);
        _bookingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);
        _bookingRoomRepositoryMock.Setup(x => x.AddAsync(It.IsAny<BookingRoom>())).ReturnsAsync(new BookingRoom { Id = Guid.NewGuid() });
        _bookingRoomNightRepositoryMock.Setup(x => x.AddRangeAsync(It.IsAny<IEnumerable<BookingRoomNight>>())).Returns(Task.CompletedTask);
        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>())).ReturnsAsync(booking);

        // Act
        var result = await _bookingService.CreateBookingAsync(request);

        // Assert
        result.Should().NotBeNull();
        // O preço deve ser 200.00 (basePrice) × 2 noites = 400.00
        result!.TotalAmount.Should().Be(400.00m);
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
    public async Task CancelBookingAsync_ShouldDeleteBookingRoomNights()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var bookingRoomId = Guid.NewGuid();
        
        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = bookingId,
            Status = "CONFIRMED",
            BookingRooms = new List<BookingRoom>
            {
                new() { Id = bookingRoomId, RoomId = Guid.NewGuid() }
            }
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);
        _bookingRoomNightRepositoryMock.Setup(x => x.DeleteByBookingRoomIdAsync(bookingRoomId)).Returns(Task.CompletedTask);

        // Act
        var result = await _bookingService.CancelBookingAsync(bookingId, "Test cancellation");

        // Assert
        result.Should().BeTrue();
        _bookingRoomNightRepositoryMock.Verify(x => x.DeleteByBookingRoomIdAsync(bookingRoomId), Times.Once);
        _bookingRepositoryMock.Verify(x => x.UpdateAsync(It.Is<AvenSuitesApi.Domain.Entities.Booking>(b => b.Status == "CANCELLED")), Times.Once);
    }
}

