using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Rooms;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly IRoomTypeService _roomTypeService;

    public RoomTypesController(IRoomTypeService roomTypeService)
    {
        _roomTypeService = roomTypeService;
    }

    [HttpPost]
    public async Task<ActionResult<RoomTypeResponse>> Create([FromBody] RoomTypeCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var roomType = await _roomTypeService.CreateRoomTypeAsync(request);
        if (roomType == null)
            return BadRequest("Não foi possível criar o tipo de quarto.");

        return CreatedAtAction(nameof(GetById), new { id = roomType.Id }, roomType);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RoomTypeResponse>> GetById(Guid id)
    {
        var roomType = await _roomTypeService.GetRoomTypeByIdAsync(id);
        if (roomType == null)
            return NotFound();

        return Ok(roomType);
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<RoomTypeResponse>>> GetByHotel(Guid hotelId, [FromQuery] bool activeOnly = true)
    {
        var roomTypes = await _roomTypeService.GetRoomTypesByHotelAsync(hotelId, activeOnly);
        return Ok(roomTypes);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RoomTypeResponse>> Update(Guid id, [FromBody] RoomTypeCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var roomType = await _roomTypeService.UpdateRoomTypeAsync(id, request);
        if (roomType == null)
            return NotFound();

        return Ok(roomType);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roomTypeService.DeleteRoomTypeAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

