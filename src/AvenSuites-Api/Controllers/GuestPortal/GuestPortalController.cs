using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.GuestPortal;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Guest")]  // Apenas Guest
public class GuestPortalController : ControllerBase
{
    private readonly IGuestRegistrationService _guestRegistrationService;
    private readonly IBookingService _bookingService;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<GuestPortalController> _logger;

    public GuestPortalController(
        IGuestRegistrationService guestRegistrationService,
        IBookingService bookingService,
        ICurrentUserService currentUser,
        ILogger<GuestPortalController> logger)
    {
        _guestRegistrationService = guestRegistrationService;
        _bookingService = bookingService;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>
    /// Obter perfil do hóspede logado
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<GuestProfileResponse>> GetProfile()
    {
        try
        {
            var guestId = _currentUser.GetUserGuestId();
            if (!guestId.HasValue)
            {
                return BadRequest(new { message = "GuestId não encontrado no token" });
            }

            var profile = await _guestRegistrationService.GetProfileAsync(guestId.Value);
            return Ok(profile);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter perfil do hóspede");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Atualizar perfil do hóspede logado
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<GuestProfileResponse>> UpdateProfile([FromBody] GuestRegisterRequest request)
    {
        try
        {
            var guestId = _currentUser.GetUserGuestId();
            if (!guestId.HasValue)
            {
                return BadRequest(new { message = "GuestId não encontrado no token" });
            }

            var profile = await _guestRegistrationService.UpdateProfileAsync(guestId.Value, request);
            return Ok(profile);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar perfil do hóspede");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Listar reservas do hóspede logado
    /// </summary>
    [HttpGet("bookings")]
    public async Task<IActionResult> GetMyBookings()
    {
        try
        {
            var guestId = _currentUser.GetUserGuestId();
            if (!guestId.HasValue)
            {
                return BadRequest(new { message = "GuestId não encontrado no token" });
            }

            var bookings = await _bookingService.GetBookingsByGuestAsync(guestId.Value);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar reservas do hóspede");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obter detalhes de uma reserva específica do hóspede logado
    /// </summary>
    [HttpGet("bookings/{id}")]
    public async Task<IActionResult> GetMyBooking(Guid id)
    {
        try
        {
            var guestId = _currentUser.GetUserGuestId();
            if (!guestId.HasValue)
            {
                return BadRequest(new { message = "GuestId não encontrado no token" });
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Reserva não encontrada" });
            }

            // Verificar se a reserva pertence ao hóspede logado
            if (booking.MainGuestId != guestId.Value)
            {
                return Forbid();
            }

            return Ok(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter reserva do hóspede");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Cancelar uma reserva
    /// </summary>
    [HttpPost("bookings/{id}/cancel")]
    public async Task<IActionResult> CancelBooking(Guid id, [FromBody] string? reason = null)
    {
        try
        {
            var guestId = _currentUser.GetUserGuestId();
            if (!guestId.HasValue)
            {
                return BadRequest(new { message = "GuestId não encontrado no token" });
            }

            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Reserva não encontrada" });
            }

            // Verificar se a reserva pertence ao hóspede logado
            if (booking.MainGuestId != guestId.Value)
            {
                return Forbid();
            }

            var success = await _bookingService.CancelBookingAsync(id, reason);
            if (!success)
            {
                return BadRequest(new { message = "Não foi possível cancelar a reserva" });
            }

            return Ok(new { message = "Reserva cancelada com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar reserva");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
}

