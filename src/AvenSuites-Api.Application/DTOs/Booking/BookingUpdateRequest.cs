using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Booking;

public class BookingUpdateRequest
{
    [MaxLength(30)]
    public string? Status { get; set; }
    
    public DateTime? CheckInDate { get; set; }
    
    public DateTime? CheckOutDate { get; set; }
    
    [Range(1, 20)]
    public short? Adults { get; set; }
    
    public short? Children { get; set; }
    
    public string? Notes { get; set; }
}

