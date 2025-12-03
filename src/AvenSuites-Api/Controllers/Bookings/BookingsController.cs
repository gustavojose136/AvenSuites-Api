using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AvenSuitesApi.Application.DTOs.Booking;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Controllers.Bookings;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly ICurrentUserService _currentUser;

    public BookingsController(
        IBookingService bookingService,
        ICurrentUserService currentUser)
    {
        _bookingService = bookingService;
        _currentUser = currentUser;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), 200)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetAll(
        [FromQuery] Guid? hotelId = null,
        [FromQuery] Guid? guestId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        if (_currentUser.IsAdmin())
        {
            if (hotelId.HasValue)
            {
                var bookings = await _bookingService.GetBookingsByHotelAsync(hotelId.Value, startDate, endDate);
                return Ok(bookings);
            }
            
            if (guestId.HasValue)
            {
                var bookings = await _bookingService.GetBookingsByGuestAsync(guestId.Value);
                return Ok(bookings);
            }
            
            return BadRequest(new { message = "Especifique hotelId ou guestId para listar reservas" });
        }
        
        if (_currentUser.IsHotelAdmin())
        {
            var userHotelId = _currentUser.GetUserHotelId();
            if (!userHotelId.HasValue)
                return Forbid();
            
            if (hotelId.HasValue && hotelId.Value != userHotelId.Value)
                return Forbid();
            
            var bookings = await _bookingService.GetBookingsByHotelAsync(userHotelId.Value, startDate, endDate);
            return Ok(bookings);
        }
        
        return Forbid();
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(BookingResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<BookingResponse>> GetById(Guid id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        if (!_currentUser.HasAccessToHotel(booking.HotelId))
            return Forbid();

        return Ok(booking);
    }

    [HttpGet("code/{code}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(BookingResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<BookingResponse>> GetByCode(Guid hotelId, string code)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        var booking = await _bookingService.GetBookingByCodeAsync(hotelId, code);
        if (booking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        return Ok(booking);
    }

    [HttpGet("hotel/{hotelId}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), 200)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByHotel(
        Guid hotelId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(hotelId))
            return Forbid();

        var bookings = await _bookingService.GetBookingsByHotelAsync(hotelId, startDate, endDate);
        return Ok(bookings);
    }

    [HttpGet("guest/{guestId}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), 200)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByGuest(Guid guestId)
    {
        var bookings = await _bookingService.GetBookingsByGuestAsync(guestId);
        
        if (_currentUser.IsHotelAdmin())
        {
            var userHotelId = _currentUser.GetUserHotelId();
            if (userHotelId.HasValue)
            {
                bookings = bookings.Where(b => b.HotelId == userHotelId.Value).ToList();
            }
        }
        
        return Ok(bookings);
    }

    [HttpPost]
    [Authorize()]
    [ProducesResponseType(typeof(BookingResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<BookingResponse>> Create([FromBody] BookingCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(request.HotelId))
            return Forbid();

        var booking = await _bookingService.CreateBookingAsync(request);
        if (booking == null)
            return BadRequest(new { message = "Não foi possível criar a reserva." });

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(BookingResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult<BookingResponse>> Update(Guid id, [FromBody] BookingUpdateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existingBooking = await _bookingService.GetBookingByIdAsync(id);
        if (existingBooking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(existingBooking.HotelId))
            return Forbid();

        var booking = await _bookingService.UpdateBookingAsync(id, request);
        if (booking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        return Ok(booking);
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] string? reason = null)
    {
        var existingBooking = await _bookingService.GetBookingByIdAsync(id);
        if (existingBooking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(existingBooking.HotelId))
            return Forbid();

        var result = await _bookingService.CancelBookingAsync(id, reason);
        if (!result)
            return NotFound(new { message = "Reserva não encontrada" });

        return Ok(new { message = "Reserva cancelada com sucesso." });
    }

    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var existingBooking = await _bookingService.GetBookingByIdAsync(id);
        if (existingBooking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(existingBooking.HotelId))
            return Forbid();

        var result = await _bookingService.ConfirmBookingAsync(id);
        if (!result)
            return NotFound(new { message = "Reserva não encontrada" });

        return Ok(new { message = "Reserva confirmada com sucesso." });
    }

    [HttpPost("{id}/check-in")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> CheckIn(Guid id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        if (!_currentUser.HasAccessToHotel(booking.HotelId))
            return Forbid();

        var result = await _bookingService.CheckInAsync(id);
        if (!result)
            return BadRequest(new { message = "Não foi possível realizar check-in" });

        var updatedBooking = await _bookingService.GetBookingByIdAsync(id);
        return Ok(new { message = "Check-in realizado com sucesso", booking = updatedBooking });
    }

    [HttpPost("{id}/check-out")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(403)]
    public async Task<ActionResult> CheckOut(Guid id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null)
            return NotFound(new { message = "Reserva não encontrada" });

        if (!_currentUser.HasAccessToHotel(booking.HotelId))
            return Forbid();

        var result = await _bookingService.CheckOutAsync(id);
        if (!result)
            return BadRequest(new { message = "Não foi possível realizar check-out" });

        var updatedBooking = await _bookingService.GetBookingByIdAsync(id);
        return Ok(new { message = "Check-out realizado com sucesso", booking = updatedBooking });
    }
}
