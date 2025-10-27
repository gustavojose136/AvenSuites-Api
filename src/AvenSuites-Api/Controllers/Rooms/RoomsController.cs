using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Rooms;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost]
    public async Task<ActionResult<RoomResponse>> Create([FromBody] RoomCreateRequest request)
    {
        var room = await _roomService.CreateRoomAsync(request);
        if (room == null)
            return BadRequest("Não foi possível criar o quarto.");

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomResponse>> GetById(Guid id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetByHotel(Guid hotelId, [FromQuery] string? status = null)
    {
        var rooms = await _roomService.GetRoomsByHotelAsync(hotelId, status);
        return Ok(rooms);
    }

    [HttpGet("availability")]
    public async Task<ActionResult<IEnumerable<RoomAvailabilityResponse>>> CheckAvailability([FromQuery] RoomAvailabilityRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var rooms = await _roomService.CheckAvailabilityAsync(request);
        return Ok(rooms);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomResponse>> Update(Guid id, [FromBody] RoomUpdateRequest request)
    {
        var room = await _roomService.UpdateRoomAsync(id, request);
        if (room == null)
            return NotFound();

        return Ok(room);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roomService.DeleteRoomAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

