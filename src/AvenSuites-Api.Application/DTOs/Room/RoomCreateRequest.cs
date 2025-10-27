using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Room;

public class RoomCreateRequest
{
    [Required]
    public Guid HotelId { get; set; }
    
    [Required]
    public Guid RoomTypeId { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string RoomNumber { get; set; } = string.Empty;
    
    [MaxLength(10)]
    public string? Floor { get; set; }
    
    [MaxLength(30)]
    public string Status { get; set; } = "ACTIVE";
}

