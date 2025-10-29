using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Invoice;

/// <summary>
/// DTO simplificado para criação de NF-e - preenche automaticamente dados do hotel e IPM
/// </summary>
public class SimpleInvoiceCreateRequest
{
    [Required]
    public Guid RoomId { get; set; }
    
    [Required]
    [Range(1, 365)]
    public int Days { get; set; }
    
    [Required]
    [Range(1, 20)]
    public int Adults { get; set; }
    
    [Range(0, 10)]
    public int Children { get; set; } = 0;
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public Guid GuestId { get; set; }
    
    [Required]
    public DateTime CheckInDate { get; set; }
    
    [Required]
    public DateTime CheckOutDate { get; set; }
    
    [Required]
    [Range(0.01, 999999.99)]
    public decimal TotalValue { get; set; }
    
    [MaxLength(500)]
    public string? Observations { get; set; }
}

