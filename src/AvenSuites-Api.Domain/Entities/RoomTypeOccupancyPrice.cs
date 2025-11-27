using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Domain.Entities;

/// <summary>
/// Representa o preço de um tipo de quarto baseado no número de hóspedes (ocupação).
/// Permite definir preços diferentes para 1, 2, 3 ou mais hóspedes.
/// </summary>
public class RoomTypeOccupancyPrice
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid RoomTypeId { get; set; }
    
    /// <summary>
    /// Número de hóspedes para este preço (1, 2, 3, etc.)
    /// </summary>
    [Required]
    [Range(1, 20)]
    public short Occupancy { get; set; }
    
    /// <summary>
    /// Preço por noite para esta ocupação
    /// </summary>
    [Required]
    [Range(0, 999999999999.99)]
    public decimal PricePerNight { get; set; } = 0.00m;
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property
    public virtual RoomType RoomType { get; set; } = null!;
}

