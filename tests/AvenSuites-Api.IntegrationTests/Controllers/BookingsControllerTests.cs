using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using AvenSuitesApi.Application.DTOs.Booking;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.IntegrationTests.Controllers;

public class BookingsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BookingsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_Bookings_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new BookingCreateRequest
        {
            HotelId = Guid.NewGuid(),
            Code = "RES-001",
            Source = "WEB",
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            Adults = 2,
            Children = 0,
            Currency = "BRL",
            MainGuestId = Guid.NewGuid()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/bookings", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Get_Bookings_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/bookings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}

