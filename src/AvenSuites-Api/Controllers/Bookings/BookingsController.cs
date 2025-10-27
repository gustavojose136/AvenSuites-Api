using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Booking;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Bookings;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    [HttpPost]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] BookingCreateRequest request)
    {
        var booking = await _bookingService.CreateBookingAsync(request);
        if (booking == null)
            return BadRequest("Não foi possível criar a reserva.");

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpGet("code/{code}")]
    public async Task<ActionResult<BookingResponse>> GetByCode(Guid hotelId, string code)
    {
        var booking = await _bookingService.GetBookingByCodeAsync(hotelId, code);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByHotel(Guid hotelId, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
    {
        var bookings = await _bookingService.GetBookingsByHotelAsync(hotelId, startDate, endDate);
        return Ok(bookings);
    }

    [HttpGet("guest/{guestId}")]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByGuest(Guid guestId)
    {
        var bookings = await _bookingService.GetBookingsByGuestAsync(guestId);
        return Ok(bookings);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BookingResponse>> Update(Guid id, [FromBody] BookingUpdateRequest request)
    {
        var booking = await _bookingService.UpdateBookingAsync(id, request);
        if (booking == null)
            return NotFound();

        return Ok(booking);
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] string? reason = null)
    {
        var result = await _bookingService.CancelBookingAsync(id, reason);
        if (!result)
            return NotFound();

        return Ok(new { message = "Reserva cancelada com sucesso." });
    }

    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var result = await _bookingService.ConfirmBookingAsync(id);
        if (!result)
            return NotFound();

        return Ok(new { message = "Reserva confirmada com sucesso." });
    }
}

