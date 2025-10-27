using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Room;

public class RoomAvailabilityRequest
{
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    public DateTime CheckInDate { get; set; }
    
    [Required]
    public DateTime CheckOutDate { get; set; }
    
    public Guid? RoomTypeId { get; set; }
    
    public short Adults { get; set; } = 2;
    
    public short Children { get; set; } = 0;
}

