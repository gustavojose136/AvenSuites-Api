using AvenSuitesApi.Application.DTOs;
using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Implementations.Guest;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Application.Utils;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;

namespace AvenSuitesApi.Application.Tests.Services.Guest;

public class GuestRegistrationServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IGuestRepository> _guestRepositoryMock;
    private readonly Mock<IGuestPiiRepository> _guestPiiRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<ILogger<GuestRegistrationService>> _loggerMock;
    private readonly GuestRegistrationService _guestRegistrationService;

    public GuestRegistrationServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _guestRepositoryMock = new Mock<IGuestRepository>();
        _guestPiiRepositoryMock = new Mock<IGuestPiiRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _jwtServiceMock = new Mock<IJwtService>();
        _emailServiceMock = new Mock<IEmailService>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<GuestRegistrationService>>();

        _guestRegistrationService = new GuestRegistrationService(
            _userRepositoryMock.Object,
            _guestRepositoryMock.Object,
            _guestPiiRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _jwtServiceMock.Object,
            _emailServiceMock.Object,
            _emailTemplateServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidRequest_ShouldReturnLoginResponse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var guestId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var request = new GuestRegisterRequest
        {
            Name = "Test Guest",
            Email = "guest@test.com",
            Password = "Password123!",
            Phone = "+5511999999999",
            DocumentType = "CPF",
            Document = "12345678901",
            BirthDate = DateTime.Now.AddYears(-30),
            AddressLine1 = "Rua Teste",
            City = "São Paulo",
            State = "SP",
            PostalCode = "01234567",
            CountryCode = "BR",
            HotelId = hotelId,
            MarketingConsent = true
        };

        var hotel = new Hotel
        {
            Id = hotelId,
            Name = "Test Hotel",
            Cnpj = "12345678901234"
        };

        var guestRole = new Role
        {
            Id = roleId,
            Name = "Guest"
        };

        var user = new User
        {
            Id = userId,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = Argon2PasswordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var guest = new AvenSuitesApi.Domain.Entities.Guest
        {
            Id = guestId,
            HotelId = hotelId,
            UserId = userId,
            MarketingConsent = true,
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId))
            .ReturnsAsync(hotel);
        _roleRepositoryMock.Setup(x => x.GetByNameAsync("Guest"))
            .ReturnsAsync(guestRole);
        _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>()))
            .ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(user);
        _guestRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>()))
            .ReturnsAsync(guest);
        _guestPiiRepositoryMock.Setup(x => x.AddOrUpdateAsync(It.IsAny<GuestPii>()))
            .Returns(Task.CompletedTask);
        _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns("test_token");
        _emailTemplateServiceMock.Setup(x => x.GenerateWelcomeEmail(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("email_body");
        _emailServiceMock.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
            It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _guestRegistrationService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("test_token");
        result.User.Should().NotBeNull();
        result.User.Email.Should().Be(request.Email);
        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _guestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Guest>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ShouldThrowException()
    {
        // Arrange
        var request = new GuestRegisterRequest
        {
            Email = "existing@test.com",
            HotelId = Guid.NewGuid()
        };

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(true);

        // Act & Assert
        await _guestRegistrationService.Invoking(x => x.RegisterAsync(request))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email já cadastrado");
    }

    [Fact]
    public async Task RegisterAsync_WithInvalidHotel_ShouldThrowException()
    {
        // Arrange
        var request = new GuestRegisterRequest
        {
            Email = "test@test.com",
            HotelId = Guid.NewGuid()
        };

        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(request.Email))
            .ReturnsAsync(false);
        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(request.HotelId))
            .ReturnsAsync((Hotel?)null);

        // Act & Assert
        await _guestRegistrationService.Invoking(x => x.RegisterAsync(request))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Hotel não encontrado");
    }

    [Fact]
    public async Task GetProfileAsync_WithValidGuest_ShouldReturnProfile()
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
            Hotel = new Hotel
            {
                Id = hotelId,
                Name = "Test Hotel"
            },
            GuestPii = new GuestPii
            {
                GuestId = guestId,
                FullName = "Test Guest",
                Email = "guest@test.com",
                PhoneE164 = "+5511999999999",
                DocumentType = "CPF",
                DocumentPlain = "12345678901"
            }
        };

        _guestRepositoryMock.Setup(x => x.GetByIdWithPiiAsync(guestId))
            .ReturnsAsync(guest);

        // Act
        var result = await _guestRegistrationService.GetProfileAsync(guestId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(guestId);
        result.Name.Should().Be("Test Guest");
        result.Email.Should().Be("guest@test.com");
    }

    [Fact]
    public async Task GetProfileAsync_WithNonExistentGuest_ShouldThrowException()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        _guestRepositoryMock.Setup(x => x.GetByIdWithPiiAsync(guestId))
            .ReturnsAsync((AvenSuitesApi.Domain.Entities.Guest?)null);

        // Act & Assert
        await _guestRegistrationService.Invoking(x => x.GetProfileAsync(guestId))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Hóspede não encontrado");
    }
}





