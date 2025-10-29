using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations.Invoice;

/// <summary>
/// Serviço que gerencia credenciais IPM com criptografia automática
/// </summary>
public class IpmCredentialsService : IIpmCredentialsService
{
    private readonly IIpmCredentialsRepository _repository;
    private readonly ISecureEncryptionService _encryptionService;

    public IpmCredentialsService(
        IIpmCredentialsRepository repository,
        ISecureEncryptionService encryptionService)
    {
        _repository = repository;
        _encryptionService = encryptionService;
    }

    public async Task<IpmCredentials?> GetDecryptedByHotelIdAsync(Guid hotelId)
    {
        var credentials = await _repository.GetByHotelIdAsync(hotelId);
        
        if (credentials == null)
            return null;

        // Descriptografar senha antes de retornar
        var decryptedCredentials = CloneCredentials(credentials);
        decryptedCredentials.Password = _encryptionService.Decrypt(credentials.Password);
        
        return decryptedCredentials;
    }

    public async Task<IpmCredentials> AddAsync(IpmCredentials credentials)
    {
        // Criptografar senha antes de salvar
        var encryptedCredentials = CloneCredentials(credentials);
        encryptedCredentials.Password = _encryptionService.Encrypt(credentials.Password);
        
        return await _repository.AddAsync(encryptedCredentials);
    }

    public async Task<IpmCredentials> UpdateAsync(IpmCredentials credentials)
    {
        // Se a senha foi fornecida, criptografar antes de salvar
        var encryptedCredentials = CloneCredentials(credentials);
        
        // Verificar se a senha precisa ser criptografada (se não parece já estar criptografada)
        // Assumimos que se está vindo do serviço, é texto plano e precisa criptografar
        if (!string.IsNullOrEmpty(credentials.Password))
        {
            // Se a senha não parece ser Base64 (criptografada), vamos criptografar
            if (!IsBase64String(credentials.Password))
            {
                encryptedCredentials.Password = _encryptionService.Encrypt(credentials.Password);
            }
            // Se já está em Base64, pode já estar criptografada, manter como está
        }
        
        return await _repository.UpdateAsync(encryptedCredentials);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Clona as credenciais para evitar modificar a instância original
    /// </summary>
    private static IpmCredentials CloneCredentials(IpmCredentials original)
    {
        return new IpmCredentials
        {
            Id = original.Id,
            HotelId = original.HotelId,
            Username = original.Username,
            Password = original.Password,
            CpfCnpj = original.CpfCnpj,
            CityCode = original.CityCode,
            SerieNfse = original.SerieNfse,
            Active = original.Active,
            CreatedAt = original.CreatedAt,
            UpdatedAt = original.UpdatedAt
        };
    }

    /// <summary>
    /// Verifica se uma string é Base64 válida
    /// </summary>
    private static bool IsBase64String(string str)
    {
        if (string.IsNullOrWhiteSpace(str))
            return false;

        try
        {
            Convert.FromBase64String(str);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

