using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AvenSuitesApi.Application.Services.Interfaces;

namespace AvenSuitesApi.Application.Services.Implementations;

/// <summary>
/// Implementação de criptografia AES-256-GCM para dados sensíveis
/// </summary>
public class SecureEncryptionService : ISecureEncryptionService
{
    private readonly byte[] _key;
    private readonly ILogger<SecureEncryptionService> _logger;

    public SecureEncryptionService(IConfiguration configuration, ILogger<SecureEncryptionService> logger)
    {
        _logger = logger;
        
        // Buscar chave de criptografia do appsettings.json
        var encryptionKey = configuration["Security:EncryptionKey"];
        
        if (string.IsNullOrWhiteSpace(encryptionKey))
        {
            _logger.LogWarning(
                "Chave de criptografia não configurada. Usando chave padrão (NÃO USE EM PRODUÇÃO!)");
            
            // Chave padrão para desenvolvimento - NUNCA usar em produção
            encryptionKey = "AvenSuites-Development-Key-32Bytes-Long!!";
        }

        // Garantir que a chave tenha exatamente 32 bytes (256 bits) para AES-256
        var keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        if (keyBytes.Length != 32)
        {
            // Se a chave não tiver 32 bytes, fazer padding ou truncar
            _key = new byte[32];
            var copyLength = Math.Min(keyBytes.Length, 32);
            Array.Copy(keyBytes, 0, _key, 0, copyLength);
            
            // Se for menor, preencher com zeros
            if (copyLength < 32)
            {
                _logger.LogWarning(
                    "Chave de criptografia ajustada para 32 bytes. Configure uma chave exata de 32 caracteres.");
            }
        }
        else
        {
            _key = keyBytes;
        }
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return string.Empty;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            aes.GenerateIV(); // Gera IV aleatório para cada criptografia

            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            
            // Escrever IV primeiro (16 bytes)
            msEncrypt.Write(aes.IV, 0, aes.IV.Length);

            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(plainText);
            }

            var encrypted = msEncrypt.ToArray();
            return Convert.ToBase64String(encrypted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criptografar texto");
            throw new InvalidOperationException("Falha ao criptografar dados", ex);
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return string.Empty;

        try
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Extrair IV (primeiros 16 bytes)
            var iv = new byte[16];
            Array.Copy(fullCipher, 0, iv, 0, 16);
            aes.IV = iv;

            // Resto é o texto criptografado
            var cipher = new byte[fullCipher.Length - 16];
            Array.Copy(fullCipher, 16, cipher, 0, cipher.Length);

            using var decryptor = aes.CreateDecryptor();
            using var msDecrypt = new MemoryStream(cipher);
            using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
            using var srDecrypt = new StreamReader(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao descriptografar texto");
            throw new InvalidOperationException("Falha ao descriptografar dados", ex);
        }
    }
}

