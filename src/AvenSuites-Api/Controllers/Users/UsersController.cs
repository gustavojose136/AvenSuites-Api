using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Controllers.Users;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
        });

        return Ok(userDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserId != id.ToString() && !User.IsInRole("Admin"))
            return Forbid();

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return Ok(userDto);
    }

    [HttpGet("profile")]
    public async Task<ActionResult<UserDto>> GetProfile()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null)
            return NotFound();

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };

        return Ok(userDto);
    }
}
