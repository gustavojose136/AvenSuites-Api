using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Booking;
using AvenSuitesApi.Application.Services.Implementations.Booking;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using RoomType = AvenSuitesApi.Domain.Entities.RoomType;
using Xunit;

namespace AvenSuites_Api.Application.Tests.Services.Booking;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IGuestRepository> _guestRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly BookingService _bookingService;

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _guestRepositoryMock = new Mock<IGuestRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();

        _bookingService = new BookingService(
            _bookingRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _guestRepositoryMock.Object,
            _roomRepositoryMock.Object);
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
            BookingRooms = new List<BookingRoomCreateRequest>
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
            TotalAmount = request.BookingRooms.Sum(br => br.PriceTotal) * (request.CheckOutDate - request.CheckInDate).Days,
            CreatedAt = DateTime.UtcNow
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _guestRepositoryMock.Setup(x => x.GetByIdAsync(guestId)).ReturnsAsync(guest);
        _roomRepositoryMock.Setup(x => x.GetByIdAsync(roomId)).ReturnsAsync(room);
        _roomRepositoryMock.Setup(x => x.IsRoomAvailableAsync(roomId, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);
        _bookingRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);

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
            CreatedAt = DateTime.UtcNow
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
            CreatedAt = DateTime.UtcNow
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdAsync(bookingId)).ReturnsAsync(booking);
        _bookingRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Booking>())).ReturnsAsync(booking);

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
            CreatedAt = DateTime.UtcNow
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

