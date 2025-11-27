using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Implementations.Guest;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Xunit;

namespace AvenSuitesApi.Application.Tests.Services.Guest;

public class GuestServiceTests
{
    private readonly Mock<IGuestRepository> _guestRepositoryMock;
    private readonly Mock<IGuestPiiRepository> _guestPiiRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly GuestService _guestService;

    public GuestServiceTests()
    {
        _guestRepositoryMock = new Mock<IGuestRepository>();
        _guestPiiRepositoryMock = new Mock<IGuestPiiRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        
        _guestService = new GuestService(
            _guestRepositoryMock.Object,
            _guestPiiRepositoryMock.Object,
            _hotelRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateGuestAsync_WithValidRequest_ShouldReturnGuestResponse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        
        var request = new GuestCreateRequest
        {
            HotelId = hotelId,
            FullName = "João Silva",
            Email = "joao@email.com",
            PhoneE164 = "+5511999999999",
            DocumentType = "CPF",
            DocumentPlain = "12345678900",
            BirthDate = new DateTime(1990, 1, 1),
            AddressLine1 = "Rua Teste, 123",
            City = "São Paulo",
            State = "SP",
            PostalCode = "01234567",
            CountryCode = "BR",
            MarketingConsent = true
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
            MarketingConsent = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            GuestPii = new GuestPii
            {
                GuestId = guestId,
                FullName = request.FullName,
                Email = request.Email
            }
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _guestPiiRepositoryMock.Setup(x => x.AddOrUpdateAsync(It.IsAny<GuestPii>())).ReturnsAsync(new GuestPii { GuestId = Guid.NewGuid() });
        _guestRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>())).ReturnsAsync(guest);

        // Act
        var result = await _guestService.CreateGuestAsync(request);

        // Assert
        result.Should().NotBeNull();
        result!.FullName.Should().Be(request.FullName);
        result.Email.Should().Be(request.Email);
        
        _hotelRepositoryMock.Verify(x => x.GetByIdAsync(hotelId), Times.Once);
        _guestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>()), Times.Once);
    }

    [Fact]
    public async Task CreateGuestAsync_WithInvalidHotel_ShouldReturnNull()
    {
        // Arrange
        var request = new GuestCreateRequest
        {
            HotelId = Guid.NewGuid(),
            FullName = "João Silva",
            Email = "joao@email.com"
        };

        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((AvenSuitesApi.Domain.Entities.Hotel?)null);

        // Act
        var result = await _guestService.CreateGuestAsync(request);

        // Assert
        result.Should().BeNull();
        _guestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>()), Times.Never);
    }

    [Fact]
    public async Task GetGuestByIdAsync_WithValidId_ShouldReturnGuestResponse()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        
        var guest = new AvenSuitesApi.Domain.Entities.Guest
        {
            Id = guestId,
            HotelId = hotelId,
            MarketingConsent = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            GuestPii = new GuestPii
            {
                GuestId = guestId,
                FullName = "João Silva",
                Email = "joao@email.com"
            }
        };

        _guestRepositoryMock.Setup(x => x.GetByIdWithPiiAsync(guestId)).ReturnsAsync(guest);

        // Act
        var result = await _guestService.GetGuestByIdAsync(guestId);

        // Assert
        result.Should().NotBeNull();
        result!.FullName.Should().Be("João Silva");
        result.Email.Should().Be("joao@email.com");
    }

    [Fact]
    public async Task GetGuestByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        
        _guestRepositoryMock.Setup(x => x.GetByIdWithPiiAsync(guestId)).ReturnsAsync((AvenSuitesApi.Domain.Entities.Guest?)null);

        // Act
        var result = await _guestService.GetGuestByIdAsync(guestId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetGuestsByHotelAsync_WithValidHotelId_ShouldReturnGuests()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var guests = new List<AvenSuitesApi.Domain.Entities.Guest>
        {
            new()
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                GuestPii = new GuestPii { FullName = "João Silva", Email = "joao@email.com" }
            },
            new()
            {
                Id = Guid.NewGuid(),
                HotelId = hotelId,
                GuestPii = new GuestPii { FullName = "Maria Santos", Email = "maria@email.com" }
            }
        };

        _guestRepositoryMock.Setup(x => x.GetByHotelIdAsync(hotelId)).ReturnsAsync(guests);

        // Act
        var result = await _guestService.GetGuestsByHotelAsync(hotelId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateGuestAsync_WithValidRequest_ShouldReturnUpdatedGuest()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        
        var existingGuest = new AvenSuitesApi.Domain.Entities.Guest
        {
            Id = guestId,
            HotelId = hotelId,
            MarketingConsent = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            GuestPii = new GuestPii
            {
                GuestId = guestId,
                FullName = "João Silva",
                Email = "joao@email.com"
            }
        };

        var request = new GuestCreateRequest
        {
            HotelId = hotelId,
            FullName = "João Silva Atualizado",
            Email = "joao.novo@email.com",
            MarketingConsent = true
        };

        _guestRepositoryMock.Setup(x => x.GetByIdWithPiiAsync(guestId)).ReturnsAsync(existingGuest);
        _guestRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>())).ReturnsAsync(existingGuest);
        _guestPiiRepositoryMock.Setup(x => x.AddOrUpdateAsync(It.IsAny<GuestPii>())).ReturnsAsync(new GuestPii { GuestId = guestId });

        // Act
        var result = await _guestService.UpdateGuestAsync(guestId, request);

        // Assert
        result.Should().NotBeNull();
        _guestRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGuestAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        var request = new GuestCreateRequest { HotelId = Guid.NewGuid() };

        _guestRepositoryMock.Setup(x => x.GetByIdWithPiiAsync(guestId)).ReturnsAsync((AvenSuitesApi.Domain.Entities.Guest?)null);

        // Act
        var result = await _guestService.UpdateGuestAsync(guestId, request);

        // Assert
        result.Should().BeNull();
        _guestRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>()), Times.Never);
    }

    [Fact]
    public async Task DeleteGuestAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        
        _guestRepositoryMock.Setup(x => x.ExistsAsync(guestId)).ReturnsAsync(true);
        _guestRepositoryMock.Setup(x => x.DeleteAsync(guestId)).Returns(Task.CompletedTask);

        // Act
        var result = await _guestService.DeleteGuestAsync(guestId);

        // Assert
        result.Should().BeTrue();
        _guestRepositoryMock.Verify(x => x.DeleteAsync(guestId), Times.Once);
    }

    [Fact]
    public async Task DeleteGuestAsync_WithInvalidId_ShouldStillReturnTrue()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        
        _guestRepositoryMock.Setup(x => x.DeleteAsync(guestId)).Returns(Task.CompletedTask);

        // Act
        var result = await _guestService.DeleteGuestAsync(guestId);

        // Assert
        // O serviço sempre retorna true, mesmo se o guest não existir
        result.Should().BeTrue();
        _guestRepositoryMock.Verify(x => x.DeleteAsync(guestId), Times.Once);
    }
}

