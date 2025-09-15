using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.LoginAsync(request);
        if (result == null)
            return Unauthorized(new { message = "Email ou senha inválidos" });

        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.RegisterAsync(request);
        if (result == null)
            return BadRequest(new { message = "Email já está em uso" });

        return Ok(result);
    }

    [HttpPost("validate")]
    public async Task<ActionResult> ValidatePassword([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var isValid = await _authService.ValidatePasswordAsync(request.Email, request.Password);
        if (!isValid)
            return Unauthorized(new { message = "Credenciais inválidas" });

        return Ok(new { message = "Credenciais válidas" });
    }
}
