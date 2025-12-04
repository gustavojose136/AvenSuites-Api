using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs;
using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IGuestRegistrationService _guestRegistrationService;

    public AuthController(IAuthService authService, IGuestRegistrationService guestRegistrationService)
    {
        _authService = authService;
        _guestRegistrationService = guestRegistrationService;
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

    [HttpPost("register-guest")]
    public async Task<ActionResult<LoginResponse>> RegisterGuest([FromBody] GuestRegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            if (request.HotelId == null || request.HotelId == Guid.Empty)
                request.HotelId = new Guid("7a326969-3bf6-40d9-96dc-1aecef585000");

            var result = await _guestRegistrationService.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { message = "Erro interno ao registrar hóspede" });
        }
    }
}
