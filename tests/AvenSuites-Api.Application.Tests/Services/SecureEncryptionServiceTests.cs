using AvenSuitesApi.Application.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace AvenSuitesApi.Application.Tests.Services;

public class SecureEncryptionServiceTests
{
    private readonly Mock<ILogger<SecureEncryptionService>> _loggerMock;
    private readonly IConfiguration _configuration;

    public SecureEncryptionServiceTests()
    {
        _loggerMock = new Mock<ILogger<SecureEncryptionService>>();
        
        var configurationDict = new Dictionary<string, string?>
        {
            { "Security:EncryptionKey", "AvenSuites-Encryption-Key-32-Chars!!" }
        };
        
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationDict)
            .Build();
    }

    [Fact]
    public void Encrypt_WithValidText_ShouldReturnEncryptedString()
    {
        // Arrange
        var service = new SecureEncryptionService(_configuration, _loggerMock.Object);
        var plainText = "Teste de criptografia";

        // Act
        var encrypted = service.Encrypt(plainText);

        // Assert
        encrypted.Should().NotBeNullOrEmpty();
        encrypted.Should().NotBe(plainText);
        encrypted.Should().MatchRegex(@"^[A-Za-z0-9+/=]+$"); // Base64 format
    }

    [Fact]
    public void Encrypt_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var service = new SecureEncryptionService(_configuration, _loggerMock.Object);

        // Act
        var encrypted = service.Encrypt(string.Empty);

        // Assert
        encrypted.Should().BeEmpty();
    }

    [Fact]
    public void Decrypt_WithEncryptedText_ShouldReturnOriginalText()
    {
        // Arrange
        var service = new SecureEncryptionService(_configuration, _loggerMock.Object);
        var plainText = "Teste de descriptografia";

        // Act
        var encrypted = service.Encrypt(plainText);
        var decrypted = service.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Decrypt_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var service = new SecureEncryptionService(_configuration, _loggerMock.Object);

        // Act
        var decrypted = service.Decrypt(string.Empty);

        // Assert
        decrypted.Should().BeEmpty();
    }

    [Fact]
    public void EncryptAndDecrypt_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var service = new SecureEncryptionService(_configuration, _loggerMock.Object);
        var plainText = "Teste com caracteres especiais: @#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var encrypted = service.Encrypt(plainText);
        var decrypted = service.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void EncryptAndDecrypt_WithUnicodeCharacters_ShouldWork()
    {
        // Arrange
        var service = new SecureEncryptionService(_configuration, _loggerMock.Object);
        var plainText = "Teste com unicode: 你好世界 🌍";

        // Act
        var encrypted = service.Encrypt(plainText);
        var decrypted = service.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_WithLongText_ShouldWork()
    {
        // Arrange
        var service = new SecureEncryptionService(_configuration, _loggerMock.Object);
        var plainText = new string('A', 10000); // 10KB of text

        // Act
        var encrypted = service.Encrypt(plainText);
        var decrypted = service.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_WithNullConfiguration_ShouldUseDefaultKey()
    {
        // Arrange
        var emptyConfig = new ConfigurationBuilder().Build();
        var service = new SecureEncryptionService(emptyConfig, _loggerMock.Object);
        var plainText = "Teste";

        // Act
        var encrypted = service.Encrypt(plainText);
        var decrypted = service.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }
}

