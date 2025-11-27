using AvenSuitesApi.Application.Services.Implementations;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AvenSuitesApi.Application.Tests.Services;

public class SecureEncryptionServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly Mock<ILogger<SecureEncryptionService>> _loggerMock;
    private readonly SecureEncryptionService _service;

    public SecureEncryptionServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<SecureEncryptionService>>();
        
        _configurationMock.Setup(x => x["Security:EncryptionKey"])
            .Returns("AvenSuites-Development-Key-32Bytes-Long!!");

        _service = new SecureEncryptionService(_configurationMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void Encrypt_WithValidText_ShouldReturnEncryptedString()
    {
        // Arrange
        var plainText = "Texto secreto para criptografar";

        // Act
        var result = _service.Encrypt(plainText);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe(plainText);
    }

    [Fact]
    public void Encrypt_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var plainText = "";

        // Act
        var result = _service.Encrypt(plainText);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Encrypt_WithNull_ShouldReturnEmptyString()
    {
        // Arrange & Act
        var result = _service.Encrypt(null!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Decrypt_WithEncryptedText_ShouldReturnOriginalText()
    {
        // Arrange
        var plainText = "Texto original para testar";

        // Act
        var encrypted = _service.Encrypt(plainText);
        var decrypted = _service.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Decrypt_WithEmptyString_ShouldReturnEmptyString()
    {
        // Arrange
        var encrypted = "";

        // Act
        var result = _service.Decrypt(encrypted);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void EncryptAndDecrypt_WithDifferentTexts_ShouldWorkCorrectly()
    {
        // Arrange
        var texts = new[] { "Texto 1", "Outro texto", "123456", "Texto com acentuação: áéíóú" };

        foreach (var text in texts)
        {
            // Act
            var encrypted = _service.Encrypt(text);
            var decrypted = _service.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(text);
        }
    }
}

