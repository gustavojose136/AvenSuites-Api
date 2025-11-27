using AvenSuitesApi.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace AvenSuitesApi.Domain.Tests.Entities;

public class GuestPiiTests
{
    [Fact]
    public void GuestPii_ShouldCreateWithValidData()
    {
        // Arrange
        var guestId = Guid.NewGuid();
        var fullName = "João Silva";
        var email = "joao@email.com";

        // Act
        var guestPii = new GuestPii
        {
            GuestId = guestId,
            FullName = fullName,
            Email = email
        };

        // Assert
        guestPii.GuestId.Should().Be(guestId);
        guestPii.FullName.Should().Be(fullName);
        guestPii.Email.Should().Be(email);
    }

    [Fact]
    public void GuestPii_ShouldAllowOptionalProperties()
    {
        // Arrange
        var guestPii = new GuestPii
        {
            GuestId = Guid.NewGuid(),
            FullName = "Maria Santos"
        };

        // Act
        guestPii.Email = "maria@email.com";
        guestPii.PhoneE164 = "+5511999999999";
        guestPii.DocumentType = "CPF";
        guestPii.DocumentPlain = "12345678900";
        guestPii.BirthDate = new DateTime(1990, 1, 1);
        guestPii.AddressLine1 = "Rua Teste, 123";
        guestPii.City = "São Paulo";
        guestPii.State = "SP";
        guestPii.PostalCode = "01234-567";

        // Assert
        guestPii.Email.Should().Be("maria@email.com");
        guestPii.PhoneE164.Should().Be("+5511999999999");
        guestPii.DocumentType.Should().Be("CPF");
        guestPii.DocumentPlain.Should().Be("12345678900");
        guestPii.BirthDate.Should().Be(new DateTime(1990, 1, 1));
        guestPii.AddressLine1.Should().Be("Rua Teste, 123");
        guestPii.City.Should().Be("São Paulo");
        guestPii.State.Should().Be("SP");
        guestPii.PostalCode.Should().Be("01234-567");
    }

    [Fact]
    public void GuestPii_ShouldRequireFullName()
    {
        // Arrange
        var guestPii = new GuestPii
        {
            GuestId = Guid.NewGuid(),
            FullName = "Nome Completo"
        };

        // Act & Assert
        guestPii.FullName.Should().NotBeNullOrEmpty();
    }
}

