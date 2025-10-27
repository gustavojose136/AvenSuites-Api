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

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    [HttpPost]
    public async Task<ActionResult<HotelResponse>> Create([FromBody] HotelCreateRequest request)
    {
        var hotel = await _hotelService.CreateHotelAsync(request);
        if (hotel == null)
            return BadRequest("Não foi possível criar o hotel.");

        return CreatedAtAction(nameof(GetById), new { id = hotel.Id }, hotel);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HotelResponse>> GetById(Guid id)
    {
        var hotel = await _hotelService.GetHotelByIdAsync(id);
        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    [HttpGet("cnpj/{cnpj}")]
    public async Task<ActionResult<HotelResponse>> GetByCnpj(string cnpj)
    {
        var hotel = await _hotelService.GetHotelByCnpjAsync(cnpj);
        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelResponse>>> GetAll()
    {
        var hotels = await _hotelService.GetAllHotelsAsync();
        return Ok(hotels);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<HotelResponse>> Update(Guid id, [FromBody] HotelCreateRequest request)
    {
        var hotel = await _hotelService.UpdateHotelAsync(id, request);
        if (hotel == null)
            return NotFound();

        return Ok(hotel);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _hotelService.DeleteHotelAsync(id);
        if (!result)
            return NotFound();

        return NoContent();
    }
}

