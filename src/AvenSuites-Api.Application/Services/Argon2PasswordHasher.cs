using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using System.Text;

namespace AvenSuitesApi.Application.Services;

public static class Argon2PasswordHasher
{
    private const int MemorySize = 65536; // 64 MB em KB
    private const int Iterations = 3; // Número de iterações
    private const int Parallelism = 4; // Número de threads paralelas
    private const int HashLength = 32; // Tamanho do hash em bytes

    /// <summary>
    /// Gera um hash Argon2 da senha fornecida
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <returns>Hash da senha no formato Argon2</returns>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        var passwordBytes = Encoding.UTF8.GetBytes(password);
        
        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,
            Version = Argon2Version.Nineteen,
            TimeCost = Iterations,
            MemoryCost = MemorySize,
            Lanes = Parallelism,
            Threads = Parallelism,
            Password = passwordBytes,
            Salt = GenerateSalt(),
            Secret = null,
            AssociatedData = null,
            HashLength = HashLength
        };

        var argon2 = new Argon2(config);
        using var hash = argon2.Hash();
        
        return config.EncodeString(hash.Buffer);
    }

    /// <summary>
    /// Verifica se a senha corresponde ao hash
    /// </summary>
    /// <param name="password">Senha em texto plano</param>
    /// <param name="hashedPassword">Hash armazenado</param>
    /// <returns>True se a senha estiver correta</returns>
    public static bool VerifyPassword(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            return false;

        try
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            
            var config = new Argon2Config
            {
                Type = Argon2Type.DataIndependentAddressing,
                Version = Argon2Version.Nineteen,
                TimeCost = Iterations,
                MemoryCost = MemorySize,
                Lanes = Parallelism,
                Threads = Parallelism,
                Password = passwordBytes,
                HashLength = HashLength
            };

            config.DecodeString(hashedPassword, out var salt);
            config.Salt = salt.Buffer;

            var argon2 = new Argon2(config);
            using var hash = argon2.Hash();
            
            var computedHashString = config.EncodeString(hash.Buffer);
            
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(hashedPassword),
                Encoding.UTF8.GetBytes(computedHashString)
            );
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gera um salt aleatório
    /// </summary>
    /// <returns>Salt de 16 bytes</returns>
    private static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(salt);
        return salt;
    }
}

