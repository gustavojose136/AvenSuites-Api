using System.Security.Cryptography;
using System.Text;
using Isopoh.Cryptography.Argon2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AvenSuitesApi.Security.PasswordHashing
{
    /// <summary>Contrato de hashing de senhas.</summary>
    public interface IPasswordHasher
    {
        /// <summary>Gera um hash no formato codificado ($argon2id$v=19$m=...,t=...,p=...$salt$hash).</summary>
        string Hash(string password);

        /// <summary>Valida a senha em texto contra um hash Argon2 codificado.</summary>
        bool Verify(string password, string encodedHash);

        /// <summary>Indica se o hash antigo deveria ser refeito com a política atual (mais forte).</summary>
        bool RehashNeeded(string encodedHash);
    }

    /// <summary>Opções de Argon2 (valores padrão seguros). MemoryCost é em KiB.</summary>
    public sealed class Argon2Options
    {
        /// <summary>Memória (KiB). 65536 = 64 MiB. Produção: aumente conforme hardware.</summary>
        public int MemoryCostKiB { get; set; } = 65536;

        /// <summary>Time cost (t). Normalmente 2–4.</summary>
        public int TimeCost { get; set; } = 3;

        /// <summary>Paralelismo (p). Use <= Environment.ProcessorCount.</summary>
        public int Parallelism { get; set; } = Math.Min(Environment.ProcessorCount, 4);

        /// <summary>Tamanho do hash (bytes).</summary>
        public int HashLength { get; set; } = 32;

        /// <summary>Tamanho do salt (bytes).</summary>
        public int SaltLength { get; set; } = 16;

        /// <summary>Se true, tenta configurações mais leves em Development (útil para testes locais).</summary>
        public bool UseDevRelaxedSettings { get; set; } = false;

        internal void NormalizeForEnvironment(string? environmentName)
        {
            // Sanitiza paralelismo
            if (Parallelism <= 0 || Parallelism > Environment.ProcessorCount)
                Parallelism = Math.Min(Environment.ProcessorCount, 4);

            // Em desenvolvimento, opcionalmente relaxa para acelerar testes
            if (UseDevRelaxedSettings && string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                MemoryCostKiB = Math.Min(MemoryCostKiB, 32768); // 32 MiB
                TimeCost = Math.Min(TimeCost, 2);
                Parallelism = Math.Min(Parallelism, Math.Min(Environment.ProcessorCount, 2));
            }
        }
    }

    /// <summary>Implementação Argon2id (recomendada para hashing de senhas).</summary>
    public sealed class Argon2PasswordHasher : IPasswordHasher
    {
        private readonly Argon2Options _opt;

        public Argon2PasswordHasher(Argon2Options options)
            => _opt = options ?? throw new ArgumentNullException(nameof(options));

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var cfg = new Argon2Config
            {
                Type = Argon2Type.HybridAddressing,           // Argon2id
                Version = Argon2Version.Nineteen,
                TimeCost = _opt.TimeCost,
                MemoryCost = _opt.MemoryCostKiB,               // KiB
                Lanes = _opt.Parallelism,
                Threads = _opt.Parallelism,
                Password = Encoding.UTF8.GetBytes(password),
                Salt = GenerateSalt(_opt.SaltLength),
                HashLength = _opt.HashLength
            };

            using var argon2 = new Argon2(cfg);
            using var hash = argon2.Hash();
            return cfg.EncodeString(hash.Buffer);
        }

        public bool Verify(string password, string encodedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(encodedHash))
                return false;

            // Usa os parâmetros embutidos no próprio hash (evita mismatch de política).
            return Argon2.Verify(encodedHash, Encoding.UTF8.GetBytes(password));
        }

        public bool RehashNeeded(string encodedHash)
        {
            if (string.IsNullOrWhiteSpace(encodedHash))
                return true;

            var cfg = new Argon2Config();
            if (!cfg.DecodeString(encodedHash, out var decoded))
                return true;

            // Conferimos se o hash existente é Argon2id (HybridAddressing), v=19,
            // e se os custos são menores do que a política atual.
            var correctType = cfg.Type == Argon2Type.HybridAddressing;
            var correctVersion = cfg.Version == Argon2Version.Nineteen;

            var weakerThanPolicy =
                cfg.MemoryCost < _opt.MemoryCostKiB ||
                cfg.TimeCost < _opt.TimeCost ||
                cfg.Lanes != _opt.Parallelism ||
                cfg.Threads != _opt.Parallelism ||
                cfg.HashLength != _opt.HashLength;

            return !correctType || !correctVersion || weakerThanPolicy;
        }

        private static byte[] GenerateSalt(int size)
        {
            var salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return salt;
        }
    }

    /// <summary>Extensão para registrar o componente no DI.</summary>
    public static class Argon2PasswordHasherServiceCollectionExtensions
    {
        /// <summary>
        /// Registra o Argon2PasswordHasher lendo opções de "AvenSuites:Security:Argon2".
        /// Em falta, usa defaults seguros. Se estiver em Development e UseDevRelaxedSettings=true, relaxa custos.
        /// </summary>
        public static IServiceCollection AddArgon2PasswordHasher(
            this IServiceCollection services,
            IConfiguration configuration,
            string optionsSectionPath = "AvenSuites:Security:Argon2")
        {
            // Carrega as opções (seção opcional)
            var section = configuration.GetSection(optionsSectionPath);
            var opts = section.Exists() ? section.Get<Argon2Options>() ?? new Argon2Options()
                                        : new Argon2Options();

            // Ajusta para o ambiente atual (ex.: Development)
            var envName = configuration["ASPNETCORE_ENVIRONMENT"] ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            opts.NormalizeForEnvironment(envName);

            services.AddSingleton(opts);
            services.AddSingleton<IPasswordHasher, Argon2PasswordHasher>();
            return services;
        }
    }
}