using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Hotel;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Hotels;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;
    private readonly ICurrentUserService _currentUser;

    public HotelsController(
        IHotelService hotelService,
        ICurrentUserService currentUser)
    {
        _hotelService = hotelService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Lista todos os hotéis. Admin vê todos, Hotel-Admin vê apenas o próprio hotel.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Hotel-Admin,Guest")]
    [ProducesResponseType(typeof(IEnumerable<HotelResponse>), 200)]
    public async Task<ActionResult<IEnumerable<HotelResponse>>> GetAll()
    {
        if (_currentUser.IsAdmin() || _currentUser.IsGuest())
        {
            // Admin vê todos os hotéis
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(hotels);
        }
        
        if (_currentUser.IsHotelAdmin())
        {
            // Hotel-Admin vê apenas o próprio hotel
            var hotelId = _currentUser.GetUserHotelId();
            if (!hotelId.HasValue)
                return Forbid();
                
            var hotel = await _hotelService.GetHotelByIdAsync(hotelId.Value);
            if (hotel == null)
                return NotFound();
                
            return Ok(new[] { hotel });
        }
        
        return Forbid();
    }

    /// <summary>
    /// Busca hotel por ID. Requer acesso ao hotel específico.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin,Guest")]
    [ProducesResponseType(typeof(HotelResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<HotelResponse>> GetById(Guid id)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(id))
            return Forbid();

        var hotel = await _hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound(new { message = "Hotel não encontrado" });

        return Ok(hotel);
    }

    /// <summary>
    /// Busca hotel por CNPJ. Requer acesso ao hotel.
    /// </summary>
    [HttpGet("cnpj/{cnpj}")]
    [Authorize(Roles = "Admin,Hotel-Admin,Guest")]
    [ProducesResponseType(typeof(HotelResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<HotelResponse>> GetByCnpj(string cnpj)
    {
        var hotel = await _hotelService.GetHotelByCnpjAsync(cnpj);
        if (hotel == null)
            return NotFound(new { message = "Hotel não encontrado" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotel.Id))
            return Forbid();

        return Ok(hotel);
    }

    /// <summary>
    /// Cria um novo hotel. Apenas Admin pode criar.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(HotelResponse), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<HotelResponse>> Create([FromBody] HotelCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var hotel = await _hotelService.CreateHotelAsync(request);
        if (hotel == null)
            return BadRequest(new { message = "Não foi possível criar o hotel." });

        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    /// <summary>
    /// Atualiza um hotel. Requer acesso ao hotel.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(HotelResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<HotelResponse>> Update(Guid id, [FromBody] HotelCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(id))
            return Forbid();

        var hotel = await _hotelService.UpdateHotelAsync(id, request);
        if (hotel == null)
            return NotFound(new { message = "Hotel não encontrado" });

        return Ok(hotel);
    }

    /// <summary>
    /// Deleta um hotel. Apenas Admin pode deletar.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _hotelService.DeleteHotelAsync(id);
        if (!result)
            return NotFound(new { message = "Hotel não encontrado" });

        return NoContent();
    }
}
