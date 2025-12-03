using System.Security.Claims;
using AvenSuitesApi.Application.Services.Implementations;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace AvenSuitesApi.Application.Tests.Services;

public class CurrentUserServiceTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly CurrentUserService _currentUserService;

    public CurrentUserServiceTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _currentUserService = new CurrentUserService(_httpContextAccessorMock.Object);
    }

    [Fact]
    public void GetUserId_WithValidClaim_ShouldReturnUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.GetUserId();

        // Assert
        result.Should().Be(userId);
    }

    [Fact]
    public void GetUserId_WithoutClaim_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal() };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        _currentUserService.Invoking(x => x.GetUserId())
            .Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Usuário não autenticado");
    }

    [Fact]
    public void GetUserEmail_WithValidClaim_ShouldReturnEmail()
    {
        // Arrange
        var email = "test@email.com";
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, email)
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.GetUserEmail();

        // Assert
        result.Should().Be(email);
    }

    [Fact]
    public void GetUserEmail_WithoutClaim_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal() };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        _currentUserService.Invoking(x => x.GetUserEmail())
            .Should().Throw<UnauthorizedAccessException>()
            .WithMessage("Email do usuário não encontrado");
    }

    [Fact]
    public void GetUserHotelId_WithValidClaim_ShouldReturnHotelId()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new("HotelId", hotelId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.GetUserHotelId();

        // Assert
        result.Should().Be(hotelId);
    }

    [Fact]
    public void GetUserHotelId_WithoutClaim_ShouldReturnNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal() };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.GetUserHotelId();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void GetUserRoles_WithRoles_ShouldReturnRoles()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Admin"),
            new(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.GetUserRoles();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain("Admin");
        result.Should().Contain("User");
    }

    [Fact]
    public void IsAdmin_WithAdminRole_ShouldReturnTrue()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.IsAdmin();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsAdmin_WithoutAdminRole_ShouldReturnFalse()
    {
        // Arrange
        var httpContext = new DefaultHttpContext { User = new ClaimsPrincipal() };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.IsAdmin();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasAccessToHotel_AsAdmin_ShouldReturnTrue()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.HasAccessToHotel(hotelId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasAccessToHotel_AsHotelAdmin_WithMatchingHotel_ShouldReturnTrue()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Hotel-Admin"),
            new("HotelId", hotelId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.HasAccessToHotel(hotelId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasAccessToHotel_AsHotelAdmin_WithDifferentHotel_ShouldReturnFalse()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var otherHotelId = Guid.NewGuid();
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, "Hotel-Admin"),
            new("HotelId", otherHotelId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = principal };

        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _currentUserService.HasAccessToHotel(hotelId);

        // Assert
        result.Should().BeFalse();
    }
}



