using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Invoice;

public class IpmNfseCreateRequest
{
    public Guid? BookingId { get; set; }
    
    [MaxLength(50)]
    public string? Identifier { get; set; }
    
    [Required]
    [MaxLength(20)]
    public string SerieNfse { get; set; } = "1";
    
    [Required]
    public DateTime FatoGeradorDate { get; set; }
    
    [Required]
    public decimal TotalValue { get; set; }
    
    public decimal DiscountValue { get; set; } = 0.00m;
    public decimal IrValue { get; set; } = 0.00m;
    public decimal InssValue { get; set; } = 0.00m;
    public decimal SocialContributionValue { get; set; } = 0.00m;
    public decimal RpsValue { get; set; } = 0.00m;
    public decimal PisValue { get; set; } = 0.00m;
    public decimal CofinsValue { get; set; } = 0.00m;
    
    [MaxLength(500)]
    public string? Observations { get; set; }
    
    [Required]
    public Guid TomadorGuestId { get; set; }
    
    public List<IpmNfseItemRequest> Items { get; set; } = new();
}

public class IpmNfseItemRequest
{
    [MaxLength(1)]
    public string TributaMunicipioPrestador { get; set; } = "N";
    
    [Required]
    [MaxLength(20)]
    public string CodigoLocalPrestacao { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(10)]
    public string UnidadeCodigo { get; set; } = "1";
    
    [Required]
    public int UnidadeQuantidade { get; set; }
    
    [Required]
    public decimal UnidadeValorUnitario { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string CodigoItemListaServico { get; set; } = "901"; // 901 = Serviços de hospedagem
    
    [Required]
    [MaxLength(200)]
    public string Descritivo { get; set; } = "Diárias";
    
    [Required]
    public decimal AliquotaItemLista { get; set; }
    
    [Required]
    [MaxLength(10)]
    public string SituacaoTributaria { get; set; } = "0";
    
    [Required]
    public decimal ValorTributavel { get; set; }
}

