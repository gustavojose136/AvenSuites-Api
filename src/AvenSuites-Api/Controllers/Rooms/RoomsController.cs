using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Room;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Rooms;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly ICurrentUserService _currentUser;

    public RoomsController(
        IRoomService roomService,
        ICurrentUserService currentUser)
    {
        _roomService = roomService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Lista todos os quartos com filtros opcionais.
    /// Admin vê todos, Hotel-Admin vê apenas do próprio hotel.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Hotel-Admin,Guest")]
    [ProducesResponseType(typeof(IEnumerable<RoomResponse>), 200)]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetAll(
        [FromQuery] Guid? hotelId = null,
        [FromQuery] string? status = null)
    {
        if (_currentUser.IsAdmin() || _currentUser.IsGuest())
        {
            // Admin pode filtrar por qualquer hotel ou ver todos
            if (hotelId.HasValue)
            {
                var rooms = await _roomService.GetRoomsByHotelAsync(hotelId.Value, status);
                return Ok(rooms);
            }
            
            // Buscar todos os quartos de todos os hotéis
            // Por enquanto, retornar erro pedindo para especificar hotelId
            return BadRequest(new { message = "Admin deve especificar hotelId para listar quartos" });
        }
        
        if (_currentUser.IsHotelAdmin())
        {
            // Hotel-Admin vê apenas quartos do próprio hotel
            var userHotelId = _currentUser.GetUserHotelId();
            if (!userHotelId.HasValue)
                return Forbid();
            
            // Se especificou outro hotel, negar acesso
            if (hotelId.HasValue && hotelId.Value != userHotelId.Value)
                return Forbid();
            
            var rooms = await _roomService.GetRoomsByHotelAsync(userHotelId.Value, status);
            return Ok(rooms);
        }
        
        return Forbid();
    }

    /// <summary>
    /// Busca quarto por ID. Requer acesso ao hotel do quarto.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin,Guest")]
    [ProducesResponseType(typeof(RoomResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<RoomResponse>> GetById(Guid id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room == null)
            return NotFound(new { message = "Quarto não encontrado" });

        // Verificar se tem acesso ao hotel do quarto
        if (!_currentUser.HasAccessToHotel(room.HotelId))
            return Forbid();

        return Ok(room);
    }

    /// <summary>
    /// Lista quartos por hotel. Requer acesso ao hotel.
    /// </summary>
    [HttpGet("hotel/{hotelId}")]
    [Authorize(Roles = "Admin,Hotel-Admin,Guest")]
    [ProducesResponseType(typeof(IEnumerable<RoomResponse>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<RoomResponse>>> GetByHotel(
        Guid hotelId,
        [FromQuery] string? status = null)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        var rooms = await _roomService.GetRoomsByHotelAsync(hotelId, status);
        return Ok(rooms);
    }

    /// <summary>
    /// Verifica disponibilidade de quartos. Requer acesso ao hotel.
    /// </summary>
    [HttpGet("availability")]
    [Authorize(Roles = "Admin,Hotel-Admin,Guest")]
    [ProducesResponseType(typeof(IEnumerable<RoomAvailabilityResponse>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<RoomAvailabilityResponse>>> CheckAvailability(
        [FromQuery] RoomAvailabilityRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(request.HotelId))
            return Forbid();

        var rooms = await _roomService.CheckAvailabilityAsync(request);
        return Ok(rooms);
    }

    /// <summary>
    /// Cria um novo quarto. Requer acesso ao hotel.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(RoomResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<RoomResponse>> Create([FromBody] RoomCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(request.HotelId))
            return Forbid();

        var room = await _roomService.CreateRoomAsync(request);
        if (room == null)
            return BadRequest(new { message = "Não foi possível criar o quarto." });

        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    /// <summary>
    /// Atualiza um quarto. Requer acesso ao hotel.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(RoomResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<RoomResponse>> Update(Guid id, [FromBody] RoomUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingRoom = await _roomService.GetRoomByIdAsync(id);
        if (existingRoom == null)
            return NotFound(new { message = "Quarto não encontrado" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(existingRoom.HotelId))
            return Forbid();

        var room = await _roomService.UpdateRoomAsync(id, request);
        if (room == null)
            return NotFound(new { message = "Quarto não encontrado" });

        return Ok(room);
    }

    /// <summary>
    /// Deleta um quarto. Requer acesso ao hotel.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existingRoom = await _roomService.GetRoomByIdAsync(id);
        if (existingRoom == null)
            return NotFound(new { message = "Quarto não encontrado" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(existingRoom.HotelId))
            return Forbid();

        var result = await _roomService.DeleteRoomAsync(id);
        if (!result)
            return NotFound(new { message = "Quarto não encontrado" });

        return NoContent();
    }
}
