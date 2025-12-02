using AvenSuitesApi.Application.Services.Implementations.Invoice;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AvenSuitesApi.Application.Tests.Services.Invoice;

public class IpmCredentialsServiceTests
{
    private readonly Mock<IIpmCredentialsRepository> _repositoryMock;
    private readonly Mock<ISecureEncryptionService> _encryptionServiceMock;
    private readonly IpmCredentialsService _ipmCredentialsService;

    public IpmCredentialsServiceTests()
    {
        _repositoryMock = new Mock<IIpmCredentialsRepository>();
        _encryptionServiceMock = new Mock<ISecureEncryptionService>();
        _ipmCredentialsService = new IpmCredentialsService(
            _repositoryMock.Object,
            _encryptionServiceMock.Object);
    }

    [Fact]
    public async Task GetDecryptedByHotelIdAsync_WithValidCredentials_ShouldReturnDecryptedPassword()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var encryptedPassword = "encrypted_password";
        var decryptedPassword = "plain_password";
        
        var credentials = new IpmCredentials
        {
            Id = Guid.NewGuid(),
            HotelId = hotelId,
            Username = "test_user",
            Password = encryptedPassword,
            Active = true
        };

        _repositoryMock.Setup(x => x.GetByHotelIdAsync(hotelId))
            .ReturnsAsync(credentials);
        
        _encryptionServiceMock.Setup(x => x.Decrypt(encryptedPassword))
            .Returns(decryptedPassword);

        // Act
        var result = await _ipmCredentialsService.GetDecryptedByHotelIdAsync(hotelId);

        // Assert
        result.Should().NotBeNull();
        result!.Password.Should().Be(decryptedPassword);
        result.Username.Should().Be(credentials.Username);
        _encryptionServiceMock.Verify(x => x.Decrypt(encryptedPassword), Times.Once);
    }

    [Fact]
    public async Task GetDecryptedByHotelIdAsync_WithNonExistentHotel_ShouldReturnNull()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByHotelIdAsync(hotelId))
            .ReturnsAsync((IpmCredentials?)null);

        // Act
        var result = await _ipmCredentialsService.GetDecryptedByHotelIdAsync(hotelId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldEncryptPasswordBeforeSaving()
    {
        // Arrange
        var plainPassword = "plain_password";
        var encryptedPassword = "encrypted_password";
        
        var credentials = new IpmCredentials
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Username = "test_user",
            Password = plainPassword,
            Active = true
        };

        var savedCredentials = new IpmCredentials
        {
            Id = credentials.Id,
            HotelId = credentials.HotelId,
            Username = credentials.Username,
            Password = encryptedPassword,
            Active = credentials.Active
        };

        _encryptionServiceMock.Setup(x => x.Encrypt(plainPassword))
            .Returns(encryptedPassword);
        
        _repositoryMock.Setup(x => x.AddAsync(It.IsAny<IpmCredentials>()))
            .ReturnsAsync(savedCredentials);

        // Act
        var result = await _ipmCredentialsService.AddAsync(credentials);

        // Assert
        _encryptionServiceMock.Verify(x => x.Encrypt(plainPassword), Times.Once);
        _repositoryMock.Verify(x => x.AddAsync(It.Is<IpmCredentials>(c => c.Password == encryptedPassword)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithPlainPassword_ShouldEncryptBeforeSaving()
    {
        // Arrange
        var plainPassword = "new_password";
        var encryptedPassword = "encrypted_new_password";
        
        var credentials = new IpmCredentials
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Username = "test_user",
            Password = plainPassword,
            Active = true
        };

        var updatedCredentials = new IpmCredentials
        {
            Id = credentials.Id,
            HotelId = credentials.HotelId,
            Username = credentials.Username,
            Password = encryptedPassword,
            Active = credentials.Active
        };

        _encryptionServiceMock.Setup(x => x.Encrypt(plainPassword))
            .Returns(encryptedPassword);
        
        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<IpmCredentials>()))
            .ReturnsAsync(updatedCredentials);

        // Act
        var result = await _ipmCredentialsService.UpdateAsync(credentials);

        // Assert
        _encryptionServiceMock.Verify(x => x.Encrypt(plainPassword), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.Is<IpmCredentials>(c => c.Password == encryptedPassword)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithBase64Password_ShouldNotEncryptAgain()
    {
        // Arrange
        var base64Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("already_encrypted"));
        
        var credentials = new IpmCredentials
        {
            Id = Guid.NewGuid(),
            HotelId = Guid.NewGuid(),
            Username = "test_user",
            Password = base64Password,
            Active = true
        };

        _repositoryMock.Setup(x => x.UpdateAsync(It.IsAny<IpmCredentials>()))
            .ReturnsAsync(credentials);

        // Act
        var result = await _ipmCredentialsService.UpdateAsync(credentials);

        // Assert
        _encryptionServiceMock.Verify(x => x.Encrypt(It.IsAny<string>()), Times.Never);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<IpmCredentials>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(x => x.DeleteAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        await _ipmCredentialsService.DeleteAsync(id);

        // Assert
        _repositoryMock.Verify(x => x.DeleteAsync(id), Times.Once);
    }
}


