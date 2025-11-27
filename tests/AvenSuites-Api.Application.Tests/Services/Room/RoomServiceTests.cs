using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Application.Services.Implementations.Room;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using RoomType = AvenSuitesApi.Domain.Entities.RoomType;
using Xunit;

namespace AvenSuites_Api.Application.Tests.Services.Room;

public class RoomServiceTests
{
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IRoomTypeRepository> _roomTypeRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly RoomService _roomService;

    public RoomServiceTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _roomTypeRepositoryMock = new Mock<IRoomTypeRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();

        _roomService = new RoomService(
            _roomRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _roomTypeRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateRoomAsync_WithValidRequest_ShouldReturnRoomResponse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var roomTypeId = Guid.NewGuid();
        
        var request = new RoomCreateRequest
        {
            HotelId = hotelId,
            RoomTypeId = roomTypeId,
            RoomNumber = "101",
            Floor = "1",
            Status = "ACTIVE"
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = hotelId,
            Name = "Hotel Test"
        };

        var roomType = new RoomType
        {
            Id = roomTypeId,
            Name = "Standard",
            CapacityAdults = 2
        };

        var room = new AvenSuitesApi.Domain.Entities.Room
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            RoomTypeId = roomTypeId,
            RoomNumber = request.RoomNumber,
            Floor = request.Floor,
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RoomType = roomType
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _roomTypeRepositoryMock.Setup(x => x.GetByIdAsync(roomTypeId)).ReturnsAsync(roomType);
        _roomRepositoryMock.Setup(x => x.IsRoomNumberUniqueAsync(hotelId, request.RoomNumber, null)).ReturnsAsync(true);
        _roomRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Room>())).ReturnsAsync(room);

        // Act
        var result = await _roomService.CreateRoomAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.RoomNumber.Should().Be(request.RoomNumber);
        _roomRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Room>()), Times.Once);
    }

    [Fact]
    public async Task CheckAvailabilityAsync_ShouldReturnAvailableRooms()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var request = new RoomAvailabilityRequest
        {
            HotelId = hotelId,
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            Adults = 2,
            Children = 0
        };

        var room = new AvenSuitesApi.Domain.Entities.Room
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            RoomNumber = "101",
            Status = "ACTIVE",
            RoomType = new RoomType
            {
                Name = "Standard",
                CapacityAdults = 2,
                CapacityChildren = 0,
                BasePrice = 100.00m
            }
        };

        _roomRepositoryMock.Setup(x => x.GetAvailableRoomsForPeriodAsync(
            hotelId, 
            request.CheckInDate, 
            request.CheckOutDate, 
            null))
            .ReturnsAsync(new List<AvenSuitesApi.Domain.Entities.Room> { room });

        // Act
        var result = await _roomService.CheckAvailabilityAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().RoomNumber.Should().Be("101");
    }
}

