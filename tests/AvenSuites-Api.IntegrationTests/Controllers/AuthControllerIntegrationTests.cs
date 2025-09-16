using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;

namespace AvenSuitesApi.IntegrationTests.Controllers;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Add in-memory database
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });
            });
        });

        _client = _factory.CreateClient();
        
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnSuccess()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "Test User",
            Email = "test@email.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("name").GetString().Should().Be(registerRequest.Name);
        result.GetProperty("email").GetString().Should().Be(registerRequest.Email);
        result.GetProperty("roles").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Existing User",
            Email = "existing@email.com",
            PasswordHash = "hashed_password",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var registerRequest = new
        {
            Name = "New User",
            Email = "existing@email.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithPasswordMismatch_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "Test User",
            Email = "test@email.com",
            Password = "Password123!",
            ConfirmPassword = "DifferentPassword123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=4$dGVzdA$test_hash", // Simplified hash for testing
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "User" } }
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new
        {
            Email = "test@email.com",
            Password = "test_password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        
        result.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
        result.GetProperty("user").GetProperty("email").GetString().Should().Be(loginRequest.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "nonexistent@email.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInactiveUser_ShouldReturnUnauthorized()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Inactive User",
            Email = "inactive@email.com",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=4$dGVzdA$test_hash",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginRequest = new
        {
            Email = "inactive@email.com",
            Password = "test_password"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithInvalidEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "Test User",
            Email = "invalid-email",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithWeakPassword_ShouldReturnBadRequest()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "Test User",
            Email = "test@email.com",
            Password = "123",
            ConfirmPassword = "123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }
}

