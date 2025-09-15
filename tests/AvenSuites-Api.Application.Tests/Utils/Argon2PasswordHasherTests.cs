using AvenSuitesApi.Application.Utils;
using FluentAssertions;

namespace AvenSuitesApi.Application.Tests.Utils;

public class Argon2PasswordHasherTests
{
    [Fact]
    public void HashPassword_WithValidPassword_ShouldReturnHash()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash = Argon2PasswordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$argon2");
    }

    [Fact]
    public void HashPassword_WithSamePassword_ShouldReturnDifferentHashes()
    {
        // Arrange
        var password = "MySecurePassword123!";

        // Act
        var hash1 = Argon2PasswordHasher.HashPassword(password);
        var hash2 = Argon2PasswordHasher.HashPassword(password);

        // Assert
        hash1.Should().NotBe(hash2);
        hash1.Should().NotBeNullOrEmpty();
        hash2.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var hash = Argon2PasswordHasher.HashPassword(password);

        // Act
        var isValid = Argon2PasswordHasher.VerifyPassword(password, hash);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "MySecurePassword123!";
        var wrongPassword = "WrongPassword456!";
        var hash = Argon2PasswordHasher.HashPassword(password);

        // Act
        var isValid = Argon2PasswordHasher.VerifyPassword(wrongPassword, hash);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void HashPassword_WithInvalidPassword_ShouldThrowException(string? password)
    {
        // Act & Assert
        var action = () => Argon2PasswordHasher.HashPassword(password!);
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void VerifyPassword_WithInvalidPassword_ShouldReturnFalse(string? password)
    {
        // Arrange
        var hash = Argon2PasswordHasher.HashPassword("ValidPassword123!");

        // Act
        var isValid = Argon2PasswordHasher.VerifyPassword(password!, hash);

        // Assert
        isValid.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void VerifyPassword_WithInvalidHash_ShouldReturnFalse(string? hash)
    {
        // Arrange
        var password = "ValidPassword123!";

        // Act
        var isValid = Argon2PasswordHasher.VerifyPassword(password, hash!);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithMalformedHash_ShouldReturnFalse()
    {
        // Arrange
        var password = "ValidPassword123!";
        var malformedHash = "not-a-valid-hash";

        // Act
        var isValid = Argon2PasswordHasher.VerifyPassword(password, malformedHash);

        // Assert
        isValid.Should().BeFalse();
    }

    [Fact]
    public void HashPassword_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var password = "P@ssw0rd!@#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var hash = Argon2PasswordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$argon2");
    }

    [Fact]
    public void VerifyPassword_WithSpecialCharacters_ShouldWork()
    {
        // Arrange
        var password = "P@ssw0rd!@#$%^&*()_+-=[]{}|;':\",./<>?";
        var hash = Argon2PasswordHasher.HashPassword(password);

        // Act
        var isValid = Argon2PasswordHasher.VerifyPassword(password, hash);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_WithUnicodeCharacters_ShouldWork()
    {
        // Arrange
        var password = "SenhaComAcentuação123!";

        // Act
        var hash = Argon2PasswordHasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
        hash.Should().StartWith("$argon2");
    }

    [Fact]
    public void VerifyPassword_WithUnicodeCharacters_ShouldWork()
    {
        // Arrange
        var password = "SenhaComAcentuação123!";
        var hash = Argon2PasswordHasher.HashPassword(password);

        // Act
        var isValid = Argon2PasswordHasher.VerifyPassword(password, hash);

        // Assert
        isValid.Should().BeTrue();
    }
}
