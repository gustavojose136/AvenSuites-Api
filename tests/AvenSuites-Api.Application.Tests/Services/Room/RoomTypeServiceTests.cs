using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Application.Services.Implementations.Room;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Xunit;

namespace AvenSuitesApi.Application.Tests.Services.Room;

public class RoomTypeServiceTests
{
    private readonly Mock<IRoomTypeRepository> _roomTypeRepositoryMock;
    private readonly Mock<IAmenityRepository> _amenityRepositoryMock;
    private readonly RoomTypeService _roomTypeService;

    public RoomTypeServiceTests()
    {
        _roomTypeRepositoryMock = new Mock<IRoomTypeRepository>();
        _amenityRepositoryMock = new Mock<IAmenityRepository>();
        
        _roomTypeService = new RoomTypeService(
            _roomTypeRepositoryMock.Object,
            _amenityRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateRoomTypeAsync_WithValidRequest_ShouldReturnRoomTypeResponse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var request = new RoomTypeCreateRequest
        {
            HotelId = hotelId,
            Code = "STD",
            Name = "Standard",
            Description = "Quarto padrão",
            CapacityAdults = 2,
            CapacityChildren = 1,
            BasePrice = 150.00m,
            Active = true
        };

        var roomType = new AvenSuitesApi.Domain.Entities.RoomType
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CapacityAdults = request.CapacityAdults,
            CapacityChildren = request.CapacityChildren,
            BasePrice = request.BasePrice,
            Active = request.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _roomTypeRepositoryMock.Setup(x => x.GetByIdAsync(Guid.Empty)).ReturnsAsync((AvenSuitesApi.Domain.Entities.RoomType?)null);
        _roomTypeRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.RoomType>())).ReturnsAsync(roomType);

        // Act
        var result = await _roomTypeService.CreateRoomTypeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(request.Code);
        result.Name.Should().Be(request.Name);
        result.BasePrice.Should().Be(request.BasePrice);
        
        _roomTypeRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.RoomType>()), Times.Once);
    }

    [Fact]
    public async Task GetRoomTypeByIdAsync_WithValidId_ShouldReturnRoomTypeResponse()
    {
        // Arrange
        var roomTypeId = Guid.NewGuid();
        var roomType = new AvenSuitesApi.Domain.Entities.RoomType
        {
            Id = roomTypeId,
            Code = "STD",
            Name = "Standard",
            BasePrice = 150.00m,
            Active = true
        };

        _roomTypeRepositoryMock.Setup(x => x.GetByIdWithAmenitiesAsync(roomTypeId)).ReturnsAsync(roomType);

        // Act
        var result = await _roomTypeService.GetRoomTypeByIdAsync(roomTypeId);

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be("STD");
    }

    [Fact]
    public async Task GetRoomTypeByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var roomTypeId = Guid.NewGuid();
        
        _roomTypeRepositoryMock.Setup(x => x.GetByIdWithAmenitiesAsync(roomTypeId)).ReturnsAsync((AvenSuitesApi.Domain.Entities.RoomType?)null);

        // Act
        var result = await _roomTypeService.GetRoomTypeByIdAsync(roomTypeId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetRoomTypesByHotelAsync_WithActiveOnly_ShouldReturnOnlyActiveRoomTypes()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var roomTypes = new List<AvenSuitesApi.Domain.Entities.RoomType>
        {
            new() { Id = Guid.NewGuid(), HotelId = hotelId, Code = "STD", Active = true },
            new() { Id = Guid.NewGuid(), HotelId = hotelId, Code = "DLX", Active = true }
        };

        _roomTypeRepositoryMock.Setup(x => x.GetActiveByHotelIdAsync(hotelId)).ReturnsAsync(roomTypes);

        // Act
        var result = await _roomTypeService.GetRoomTypesByHotelAsync(hotelId, activeOnly: true);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateRoomTypeAsync_WithValidRequest_ShouldReturnUpdatedRoomType()
    {
        // Arrange
        var roomTypeId = Guid.NewGuid();
        var existingRoomType = new AvenSuitesApi.Domain.Entities.RoomType
        {
            Id = roomTypeId,
            Code = "STD",
            Name = "Standard",
            BasePrice = 150.00m,
            Active = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var request = new RoomTypeCreateRequest
        {
            Code = "STD",
            Name = "Standard Updated",
            BasePrice = 200.00m,
            Active = true
        };

        _roomTypeRepositoryMock.Setup(x => x.GetByIdAsync(roomTypeId)).ReturnsAsync(existingRoomType);
        _roomTypeRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.RoomType>())).ReturnsAsync(existingRoomType);

        // Act
        var result = await _roomTypeService.UpdateRoomTypeAsync(roomTypeId, request);

        // Assert
        result.Should().NotBeNull();
        _roomTypeRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.RoomType>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRoomTypeAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var roomTypeId = Guid.NewGuid();
        
        _roomTypeRepositoryMock.Setup(x => x.ExistsAsync(roomTypeId)).ReturnsAsync(true);
        _roomTypeRepositoryMock.Setup(x => x.DeleteAsync(roomTypeId)).Returns(Task.CompletedTask);

        // Act
        var result = await _roomTypeService.DeleteRoomTypeAsync(roomTypeId);

        // Assert
        result.Should().BeTrue();
        _roomTypeRepositoryMock.Verify(x => x.DeleteAsync(roomTypeId), Times.Once);
    }
}

