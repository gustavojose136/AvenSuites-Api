using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Booking;

public class BookingCreateRequest
{
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(30)]
    public string Source { get; set; } = string.Empty;
    
    [Required]
    public DateTime CheckInDate { get; set; }
    
    [Required]
    public DateTime CheckOutDate { get; set; }
    
    [Required]
    [Range(1, 20)]
    public short Adults { get; set; } = 1;
    
    public short Children { get; set; } = 0;
    
    [MaxLength(3)]
    public string Currency { get; set; } = "BRL";
    
    [Required]
    public Guid MainGuestId { get; set; }
    
    [MaxLength(120)]
    public string? ChannelRef { get; set; }
    
    public string? Notes { get; set; }
    
    public List<BookingRoomRequest> BookingRooms { get; set; } = new();
    
    public List<Guid>? AdditionalGuestIds { get; set; }
}

public class BookingRoomRequest
{
    [Required]
    public Guid RoomId { get; set; }
    
    [Required]
    public Guid RoomTypeId { get; set; }
    
    public Guid? RatePlanId { get; set; }
    
    [Required]
    public decimal PriceTotal { get; set; }
    
    public string? Notes { get; set; }
}

