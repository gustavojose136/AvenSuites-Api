using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Guest;

/// <summary>
/// Request para hóspede criar sua própria reserva
/// </summary>
public class GuestBookingRequest
{
    [Required(ErrorMessage = "RoomId é obrigatório")]
    public Guid RoomId { get; set; }

    [Required(ErrorMessage = "Data de check-in é obrigatória")]
    public DateTime CheckInDate { get; set; }

    [Required(ErrorMessage = "Data de check-out é obrigatória")]
    public DateTime CheckOutDate { get; set; }

    [Range(1, 10, ErrorMessage = "Número de adultos deve ser entre 1 e 10")]
    public int Adults { get; set; } = 1;

    [Range(0, 10, ErrorMessage = "Número de crianças deve ser entre 0 e 10")]
    public int Children { get; set; } = 0;

    [MaxLength(500)]
    public string? SpecialRequests { get; set; }
}

