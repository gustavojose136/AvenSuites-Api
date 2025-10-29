namespace AvenSuitesApi.Application.Services.Interfaces;

/// <summary>
/// Serviço para criptografia reversível de dados sensíveis (ex: senhas de API externa)
/// </summary>
public interface ISecureEncryptionService
{
    /// <summary>
    /// Criptografa uma string usando AES-256
    /// </summary>
    string Encrypt(string plainText);

    /// <summary>
    /// Descriptografa uma string criptografada
    /// </summary>
    string Decrypt(string cipherText);
}

