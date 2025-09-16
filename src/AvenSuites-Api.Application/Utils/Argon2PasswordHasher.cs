using Isopoh.Cryptography.Argon2;
using System.Security.Cryptography;
using System.Text;

namespace AvenSuitesApi.Application.Utils;

public static class Argon2PasswordHasher
{
    private const int MemorySize = 4096;   // 4 MiB em KiB (para testes)
    private const int Iterations = 2;      // para testes
    private const int Parallelism = 2;     // para testes
    private const int HashLength = 32;     // bytes

    public static string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        var config = new Argon2Config
        {
            Type = Argon2Type.DataIndependentAddressing,   // argon2i (ok p/ compatibilidade com seu hash atual)
            Version = Argon2Version.Nineteen,
            TimeCost = Iterations,
            MemoryCost = MemorySize,
            Lanes = Parallelism,
            Threads = Parallelism,
            Password = Encoding.UTF8.GetBytes(password),
            Salt = GenerateSalt(),
            HashLength = HashLength
        };

        using var argon2 = new Argon2(config);
        using var hash = argon2.Hash();
        return config.EncodeString(hash.Buffer); // $argon2i$v=19$m=...,t=...,p=...$salt$hash
    }

    public static bool VerifyPassword(string password, string encodedHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(encodedHash))
            return false;

        return Argon2.Verify(encodedHash, Encoding.UTF8.GetBytes(password));
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[16];
        RandomNumberGenerator.Fill(salt);
        return salt;
    }
}
