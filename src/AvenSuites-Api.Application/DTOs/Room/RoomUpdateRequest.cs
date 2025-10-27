using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Room;

public class RoomUpdateRequest
{
    public Guid? RoomTypeId { get; set; }
    
    [MaxLength(20)]
    public string? RoomNumber { get; set; }
    
    [MaxLength(10)]
    public string? Floor { get; set; }
    
    [MaxLength(30)]
    public string? Status { get; set; }
}

