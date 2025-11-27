using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Hotel;
using AvenSuitesApi.Application.Services.Implementations.Hotel;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Xunit;

namespace AvenSuitesApi.Application.Tests.Services.Hotel;

public class HotelServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly HotelService _hotelService;

    public HotelServiceTests()
    {
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _hotelService = new HotelService(_hotelRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateHotelAsync_WithValidRequest_ShouldReturnHotelResponse()
    {
        // Arrange
        var request = new HotelCreateRequest
        {
            Name = "Hotel Test",
            TradeName = "Hotel Test LTDA",
            Cnpj = "12345678000190",
            Email = "contato@hoteltest.com",
            PhoneE164 = "+5511999999999",
            Timezone = "America/Sao_Paulo",
            AddressLine1 = "Rua Teste, 123",
            City = "São Paulo",
            State = "SP",
            PostalCode = "01234567",
            CountryCode = "BR"
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TradeName = request.TradeName,
            Cnpj = request.Cnpj,
            Email = request.Email,
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _hotelRepositoryMock.Setup(x => x.ExistsByCnpjAsync(request.Cnpj)).ReturnsAsync(false);
        _hotelRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Hotel>())).ReturnsAsync(hotel);

        // Act
        var result = await _hotelService.CreateHotelAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be(request.Name);
        result.Cnpj.Should().Be(request.Cnpj);
        
        _hotelRepositoryMock.Verify(x => x.ExistsByCnpjAsync(request.Cnpj), Times.Once);
        _hotelRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Hotel>()), Times.Once);
    }

    [Fact]
    public async Task CreateHotelAsync_WithDuplicateCnpj_ShouldReturnNull()
    {
        // Arrange
        var request = new HotelCreateRequest
        {
            Name = "Hotel Test",
            Cnpj = "12345678000190"
        };

        _hotelRepositoryMock.Setup(x => x.ExistsByCnpjAsync(request.Cnpj)).ReturnsAsync(true);

        // Act
        var result = await _hotelService.CreateHotelAsync(request);

        // Assert
        result.Should().BeNull();
        _hotelRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Hotel>()), Times.Never);
    }

    [Fact]
    public async Task GetHotelByIdAsync_WithValidId_ShouldReturnHotelResponse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = hotelId,
            Name = "Hotel Test",
            Status = "ACTIVE"
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);

        // Act
        var result = await _hotelService.GetHotelByIdAsync(hotelId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Hotel Test");
    }

    [Fact]
    public async Task GetHotelByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        
        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync((AvenSuitesApi.Domain.Entities.Hotel?)null);

        // Act
        var result = await _hotelService.GetHotelByIdAsync(hotelId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetHotelByCnpjAsync_WithValidCnpj_ShouldReturnHotelResponse()
    {
        // Arrange
        var cnpj = "12345678000190";
        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Hotel Test",
            Cnpj = cnpj
        };

        _hotelRepositoryMock.Setup(x => x.GetByCnpjAsync(cnpj)).ReturnsAsync(hotel);

        // Act
        var result = await _hotelService.GetHotelByCnpjAsync(cnpj);

        // Assert
        result.Should().NotBeNull();
        result!.Cnpj.Should().Be(cnpj);
    }

    [Fact]
    public async Task GetAllHotelsAsync_ShouldReturnAllHotels()
    {
        // Arrange
        var hotels = new List<AvenSuitesApi.Domain.Entities.Hotel>
        {
            new() { Id = Guid.NewGuid(), Name = "Hotel 1", Status = "ACTIVE" },
            new() { Id = Guid.NewGuid(), Name = "Hotel 2", Status = "ACTIVE" }
        };

        _hotelRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(hotels);

        // Act
        var result = await _hotelService.GetAllHotelsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateHotelAsync_WithValidRequest_ShouldReturnUpdatedHotel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var existingHotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = hotelId,
            Name = "Hotel Antigo",
            Status = "ACTIVE",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var request = new HotelCreateRequest
        {
            Name = "Hotel Atualizado",
            Email = "novo@email.com"
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(existingHotel);
        _hotelRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Hotel>())).ReturnsAsync(existingHotel);

        // Act
        var result = await _hotelService.UpdateHotelAsync(hotelId, request);

        // Assert
        result.Should().NotBeNull();
        _hotelRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Hotel>()), Times.Once);
    }

    [Fact]
    public async Task UpdateHotelAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var request = new HotelCreateRequest { Name = "Hotel Test" };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync((AvenSuitesApi.Domain.Entities.Hotel?)null);

        // Act
        var result = await _hotelService.UpdateHotelAsync(hotelId, request);

        // Assert
        result.Should().BeNull();
        _hotelRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Hotel>()), Times.Never);
    }

    [Fact]
    public async Task DeleteHotelAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = hotelId,
            Name = "Hotel Test",
            Status = "ACTIVE"
        };
        
        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _hotelRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Hotel>())).ReturnsAsync(hotel);

        // Act
        var result = await _hotelService.DeleteHotelAsync(hotelId);

        // Assert
        result.Should().BeTrue();
        _hotelRepositoryMock.Verify(x => x.UpdateAsync(It.Is<AvenSuitesApi.Domain.Entities.Hotel>(h => h.Status == "INACTIVE")), Times.Once);
    }

    [Fact]
    public async Task DeleteHotelAsync_WithInvalidId_ShouldReturnFalse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        
        _hotelRepositoryMock.Setup(x => x.ExistsAsync(hotelId)).ReturnsAsync(false);

        // Act
        var result = await _hotelService.DeleteHotelAsync(hotelId);

        // Assert
        result.Should().BeFalse();
        _hotelRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}

