using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Text.Json;

namespace AvenSuitesApi.IntegrationTests.Controllers;

public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory)
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
    public async Task GetUsers_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetUsers_WithValidToken_ShouldReturnUsers()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=4$dGVzdA$test_hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Get token by logging in
        var loginRequest = new
        {
            Email = "test@email.com",
            Password = "test_password"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<JsonElement>(loginContent);
        var token = loginResult.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        result.GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetUserById_WithValidToken_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=4$dGVzdA$test_hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Get token by logging in
        var loginRequest = new
        {
            Email = "test@email.com",
            Password = "test_password"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<JsonElement>(loginContent);
        var token = loginResult.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        result.GetProperty("id").GetString().Should().Be(userId.ToString());
        result.GetProperty("name").GetString().Should().Be(user.Name);
        result.GetProperty("email").GetString().Should().Be(user.Email);
    }

    [Fact]
    public async Task GetUserById_WithNonExistingId_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@email.com",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=4$dGVzdA$test_hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Get token by logging in
        var loginRequest = new
        {
            Email = "test@email.com",
            Password = "test_password"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<JsonElement>(loginContent);
        var token = loginResult.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync($"/api/users/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetUserById_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateUser_WithValidData_ShouldReturnUpdatedUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "Original Name",
            Email = "original@email.com",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=4$dGVzdA$test_hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Get token by logging in
        var loginRequest = new
        {
            Email = "original@email.com",
            Password = "test_password"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<JsonElement>(loginContent);
        var token = loginResult.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new
        {
            Name = "Updated Name",
            Email = "updated@email.com"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/users/{userId}", updateRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(content);
        result.GetProperty("name").GetString().Should().Be("Updated Name");
        result.GetProperty("email").GetString().Should().Be("updated@email.com");
    }

    [Fact]
    public async Task DeleteUser_WithValidId_ShouldReturnNoContent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "User to Delete",
            Email = "delete@email.com",
            PasswordHash = "$argon2id$v=19$m=65536,t=3,p=4$dGVzdA$test_hash",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Get token by logging in
        var loginRequest = new
        {
            Email = "delete@email.com",
            Password = "test_password"
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var loginContent = await loginResponse.Content.ReadAsStringAsync();
        var loginResult = JsonSerializer.Deserialize<JsonElement>(loginContent);
        var token = loginResult.GetProperty("token").GetString();

        _client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.DeleteAsync($"/api/users/{userId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    public void Dispose()
    {
        _context.Dispose();
        _client.Dispose();
    }
}

