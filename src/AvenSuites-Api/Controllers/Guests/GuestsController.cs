using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Guest;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Guests;

[ApiController]
[Route("api/[controller]")]
public class GuestsController : ControllerBase
{
    private readonly IGuestService _guestService;

    public GuestsController(IGuestService guestService)
    {
        _guestService = guestService;
    }

    [HttpPost]
    public async Task<ActionResult<GuestResponse>> Create([FromBody] GuestCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var guest = await _guestService.CreateGuestAsync(request);
        if (guest == null)
            return BadRequest("Não foi possível criar o hóspede.");

        return CreatedAtAction(nameof(GetById), new { id = guest.Id }, guest);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GuestResponse>> GetById(Guid id)
    {
        var guest = await _guestService.GetGuestByIdAsync(id);
        if (guest == null)
            return NotFound();

        return Ok(guest);
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<GuestResponse>>> GetByHotel(Guid hotelId)
    {
        var guests = await _guestService.GetGuestsByHotelAsync(hotelId);
        return Ok(guests);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GuestResponse>> Update(Guid id, [FromBody] GuestCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var guest = await _guestService.UpdateGuestAsync(id, request);
        if (guest == null)
            return NotFound();

        return Ok(guest);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _guestService.DeleteGuestAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

