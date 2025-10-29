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

    /// <summary>
    /// Lista todas as reservas com filtros opcionais.
    /// Admin vê todas, Hotel-Admin vê apenas do próprio hotel.
    /// </summary>
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
            // Admin pode filtrar por qualquer hotel
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
            
            // Se especificou outro hotel, negar acesso
            if (hotelId.HasValue && hotelId.Value != userHotelId.Value)
                return Forbid();
            
            var bookings = await _bookingService.GetBookingsByHotelAsync(userHotelId.Value, startDate, endDate);
            return Ok(bookings);
        }
        
        return Forbid();
    }

    /// <summary>
    /// Busca reserva por ID. Requer acesso ao hotel da reserva.
    /// </summary>
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

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(booking.HotelId))
            return Forbid();

        return Ok(booking);
    }

    /// <summary>
    /// Busca reserva por código. Requer acesso ao hotel.
    /// </summary>
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

    /// <summary>
    /// Lista reservas por hotel. Requer acesso ao hotel.
    /// </summary>
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

    /// <summary>
    /// Lista reservas por hóspede.
    /// </summary>
    [HttpGet("guest/{guestId}")]
    [Authorize(Roles = "Admin,Hotel-Admin")]
    [ProducesResponseType(typeof(IEnumerable<BookingResponse>), 200)]
    public async Task<ActionResult<IEnumerable<BookingResponse>>> GetByGuest(Guid guestId)
    {
        var bookings = await _bookingService.GetBookingsByGuestAsync(guestId);
        
        // Filtrar apenas reservas do hotel do usuário se for Hotel-Admin
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

    /// <summary>
    /// Cria uma nova reserva. Requer acesso ao hotel.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Hotel-Admin")]
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

    /// <summary>
    /// Atualiza uma reserva. Requer acesso ao hotel.
    /// </summary>
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

    /// <summary>
    /// Cancela uma reserva. Requer acesso ao hotel.
    /// </summary>
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

    /// <summary>
    /// Confirma uma reserva. Requer acesso ao hotel.
    /// </summary>
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

    /// <summary>
    /// Realiza check-in de uma reserva. Requer acesso ao hotel.
    /// </summary>
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

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(booking.HotelId))
            return Forbid();

        var result = await _bookingService.CheckInAsync(id);
        if (!result)
            return BadRequest(new { message = "Não foi possível realizar check-in" });

        var updatedBooking = await _bookingService.GetBookingByIdAsync(id);
        return Ok(new { message = "Check-in realizado com sucesso", booking = updatedBooking });
    }

    /// <summary>
    /// Realiza check-out de uma reserva. Requer acesso ao hotel.
    /// </summary>
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

        // Verificar se tem acesso ao hotel
        if (!_currentUser.HasAccessToHotel(booking.HotelId))
            return Forbid();

        var result = await _bookingService.CheckOutAsync(id);
        if (!result)
            return BadRequest(new { message = "Não foi possível realizar check-out" });

        var updatedBooking = await _bookingService.GetBookingByIdAsync(id);
        return Ok(new { message = "Check-out realizado com sucesso", booking = updatedBooking });
    }
}
