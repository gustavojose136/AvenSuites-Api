namespace AvenSuitesApi.Application.DTOs.Booking;

public class BookingResponse
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public short Adults { get; set; }
    public short Children { get; set; }
    public string Currency { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public Guid MainGuestId { get; set; }
    public string? ChannelRef { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public GuestSummaryResponse? MainGuest { get; set; }
    public List<BookingRoomResponse> BookingRooms { get; set; } = new();
    public List<BookingPaymentResponse> Payments { get; set; } = new();
}

public class BookingRoomResponse
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal PriceTotal { get; set; }
    public string? Notes { get; set; }
}

public class BookingPaymentResponse
{
    public Guid Id { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }
}

public class GuestSummaryResponse
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}

