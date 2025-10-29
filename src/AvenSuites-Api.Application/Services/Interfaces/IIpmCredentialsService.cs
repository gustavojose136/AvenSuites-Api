using AvenSuitesApi.Domain.Entities;

namespace AvenSuitesApi.Application.Services.Interfaces;

/// <summary>
/// Serviço para gerenciar credenciais IPM com criptografia automática
/// </summary>
public interface IIpmCredentialsService
{
    /// <summary>
    /// Busca credenciais do hotel (senha descriptografada automaticamente)
    /// </summary>
    Task<IpmCredentials?> GetDecryptedByHotelIdAsync(Guid hotelId);

    /// <summary>
    /// Adiciona credenciais (senha criptografada automaticamente)
    /// </summary>
    Task<IpmCredentials> AddAsync(IpmCredentials credentials);

    /// <summary>
    /// Atualiza credenciais (senha criptografada automaticamente se fornecida)
    /// </summary>
    Task<IpmCredentials> UpdateAsync(IpmCredentials credentials);

    /// <summary>
    /// Desativa credenciais
    /// </summary>
    Task DeleteAsync(Guid id);
}

