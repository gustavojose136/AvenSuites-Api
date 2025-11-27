using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace AvenSuitesApi.IntegrationTests.Controllers;

public class RoomsControllerTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;
    private string? _authToken;

    public RoomsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_Rooms_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient();
        
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    private async Task AuthenticateAsync()
    {
        // Criar usuário admin para testes
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Name = "Admin Test",
            Email = "admin@test.com",
            PasswordHash = "$argon2i$v=19$m=4096,t=2,p=2$test$test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(adminUser);
        await _context.SaveChangesAsync();

        // Login (simulado - em testes reais você faria o login completo)
        _authToken = "test-token";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
    }

    [Fact]
    public async Task GetAll_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/rooms");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnRoom()
    {
        // Arrange
        await AuthenticateAsync();
        
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel",
            Status = "ACTIVE"
        };

        var roomType = new RoomType
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            Code = "STD",
            Name = "Standard",
            BasePrice = 150m,
            Active = true
        };

        var room = new Room
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            RoomTypeId = roomType.Id,
            RoomNumber = "101",
            Status = "ACTIVE"
        };

        _context.Hotels.Add(hotel);
        _context.RoomTypes.Add(roomType);
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/rooms/{room.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _client.Dispose();
    }
}

