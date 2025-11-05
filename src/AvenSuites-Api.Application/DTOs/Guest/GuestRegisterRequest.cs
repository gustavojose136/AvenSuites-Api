using System.ComponentModel.DataAnnotations;

namespace AvenSuitesApi.Application.DTOs.Guest;

public class GuestRegisterRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Senha é obrigatória")]
    [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Telefone é obrigatório")]
    [Phone(ErrorMessage = "Telefone inválido")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tipo de documento é obrigatório")]
    [MaxLength(10)]
    public string DocumentType { get; set; } = string.Empty; // CPF ou CNPJ

    [Required(ErrorMessage = "Documento é obrigatório")]
    [MaxLength(20)]
    public string Document { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime BirthDate { get; set; }

    [Required(ErrorMessage = "Endereço é obrigatório")]
    [MaxLength(200)]
    public string AddressLine1 { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? AddressLine2 { get; set; }

    [Required(ErrorMessage = "Cidade é obrigatória")]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Neighborhood { get; set; }

    [Required(ErrorMessage = "Estado é obrigatório")]
    [MaxLength(2)]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "CEP é obrigatório")]
    [MaxLength(10)]
    public string PostalCode { get; set; } = string.Empty;

    [MaxLength(2)]
    public string CountryCode { get; set; } = "BR";

    public bool MarketingConsent { get; set; } = false;

    [Required(ErrorMessage = "HotelId é obrigatório")]
    public Guid HotelId { get; set; }
}

