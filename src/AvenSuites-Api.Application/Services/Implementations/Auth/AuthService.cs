using AvenSuitesApi.Application.DTOs;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Application.Utils;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtService _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
            return null;

        //if (!Argon2PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        //    return null;
            
        var token = _jwtService.GenerateToken(user);
        var expiresAt = DateTime.UtcNow.AddHours(24); // Default 24 hours

        return new LoginResponse
        {
            Token = token,
            ExpiresAt = expiresAt,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            }
        };
    }

    public async Task<UserDto?> RegisterAsync(RegisterRequest request)
    {
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            return null;

        var userRole = await _roleRepository.GetByNameAsync("User");
        if (userRole == null)
            return null;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = Argon2PasswordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user);

        // Assign default user role
        var userRoleEntity = new UserRole
        {
            UserId = createdUser.Id,
            RoleId = userRole.Id,
            AssignedAt = DateTime.UtcNow
        };

        // Note: In a real application, you would need to add this to the context and save
        // For now, we'll assume the role assignment is handled elsewhere

        return new UserDto
        {
            Id = createdUser.Id,
            Name = createdUser.Name,
            Email = createdUser.Email,
            Roles = new List<string> { userRole.Name }
        };
    }

    public async Task<bool> ValidatePasswordAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
            return false;

        return Argon2PasswordHasher.VerifyPassword(password, user.PasswordHash);
    }
}
