using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Guests;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;
    private readonly ICurrentUserService _currentUser;

    public GuestsController(
        IGuestService guestService,
        ICurrentUserService currentUser)
    {
        _guestService = guestService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Lista todos os hóspedes com filtros opcionais.
    /// Admin vê todos, Hotel-Admin vê apenas do próprio hotel.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IEnumerable<GuestResponse>), 200)]
    public async Task<ActionResult<IEnumerable<GuestResponse>>> GetAll([FromQuery] Guid? hotelId = null)
    {
        if (_currentUser.IsAdmin())
        {
            if (hotelId.HasValue)
            {
                var guests = await _guestService.GetGuestsByHotelAsync(hotelId.Value);
                return Ok(guests);
            }
            
            return BadRequest(new { message = "Admin deve especificar hotelId para listar hóspedes" });
        }
        
        if (_currentUser.IsHotelAdmin())
        {
            var userHotelId = _currentUser.GetUserHotelId();
            if (!userHotelId.HasValue)
                return Forbid();
            
            if (hotelId.HasValue && hotelId.Value != userHotelId.Value)
                return Forbid();
            
            var guests = await _guestService.GetGuestsByHotelAsync(userHotelId.Value);
            return Ok(guests);
        }
        
        return Forbid();
    }

    /// <summary>
    /// Busca hóspede por ID. Requer acesso ao hotel do hóspede.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(GuestResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<GuestResponse>> GetById(Guid id)
    {
        var guest = await _guestService.GetGuestByIdAsync(id);
        if (guest == null)
            return NotFound(new { message = "Hóspede não encontrado" });

        if (!_currentUser.HasAccessToHotel(guest.HotelId))
            return Forbid();

        return Ok(guest);
    }

    /// <summary>
    /// Lista hóspedes por hotel. Requer acesso ao hotel.
    /// </summary>
    [HttpGet("hotel/{hotelId}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IEnumerable<GuestResponse>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<GuestResponse>>> GetByHotel(Guid hotelId)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        var guests = await _guestService.GetGuestsByHotelAsync(hotelId);
        return Ok(guests);
    }

    /// <summary>
    /// Cria um novo hóspede. Requer acesso ao hotel.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(GuestResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<GuestResponse>> Create([FromBody] GuestCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(request.HotelId))
            return Forbid();

        var guest = await _guestService.CreateGuestAsync(request);
        if (guest == null)
            return BadRequest(new { message = "Não foi possível criar o hóspede." });

        return CreatedAtAction(nameof(GetById), new { id = guest.Id }, guest);
    }

    /// <summary>
    /// Atualiza um hóspede. Requer acesso ao hotel.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(GuestResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<GuestResponse>> Update(Guid id, [FromBody] GuestCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingGuest = await _guestService.GetGuestByIdAsync(id);
        if (existingGuest == null)
            return NotFound(new { message = "Hóspede não encontrado" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(existingGuest.HotelId))
            return Forbid();

        var guest = await _guestService.UpdateGuestAsync(id, request);
        if (guest == null)
            return NotFound(new { message = "Hóspede não encontrado" });

        return Ok(guest);
    }

    /// <summary>
    /// Deleta um hóspede. Requer acesso ao hotel.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingGuest = await _guestService.GetGuestByIdAsync(id);
        if (existingGuest == null)
            return NotFound(new { message = "Hóspede não encontrado" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(existingGuest.HotelId))
            return Forbid();

        var result = await _guestService.DeleteGuestAsync(id);
        if (!result)
            return NotFound(new { message = "Hóspede não encontrado" });

        return NoContent();
    }
}
