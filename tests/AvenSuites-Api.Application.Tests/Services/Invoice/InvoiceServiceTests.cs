using FluentAssertions;
using Moq;
using AvenSuitesApi.Application.DTOs.Invoice;
using AvenSuitesApi.Application.Services.Implementations.Invoice;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AvenSuitesApi.Application.Tests.Services.Invoice;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
    private readonly Mock<ILogger<InvoiceService>> _loggerMock;
    private readonly InvoiceService _invoiceService;

    public InvoiceServiceTests()
    {
        _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
        _loggerMock = new Mock<ILogger<InvoiceService>>();
        
        _invoiceService = new InvoiceService(
            _invoiceRepositoryMock.Object,
            _bookingRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _emailServiceMock.Object,
            _emailTemplateServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_WithValidBooking_ShouldReturnInvoiceResponse()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        
        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = bookingId,
            HotelId = hotelId,
            Code = "RES-001",
            Status = "CONFIRMED",
            CheckInDate = DateTime.Today.AddDays(1),
            CheckOutDate = DateTime.Today.AddDays(3),
            TotalAmount = 300.00m,
            MainGuest = new AvenSuitesApi.Domain.Entities.Guest
            {
                Id = Guid.NewGuid(),
                GuestPii = new GuestPii { FullName = "João Silva", Email = "joao@email.com" }
            },
            BookingRooms = new List<BookingRoom>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    RoomId = roomId,
                    PriceTotal = 300.00m,
                    Room = new AvenSuitesApi.Domain.Entities.Room
                    {
                        Id = roomId,
                        RoomNumber = "101"
                    }
                }
            }
        };

        var hotel = new AvenSuitesApi.Domain.Entities.Hotel
        {
            Id = hotelId,
            Name = "Hotel Test",
            Email = "hotel@test.com"
        };

        var invoice = new AvenSuitesApi.Domain.Entities.Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            HotelId = hotelId,
            Status = "PENDING",
            TotalServices = 300.00m,
            TotalTaxes = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);
        _invoiceRepositoryMock.Setup(x => x.GetByBookingIdAsync(bookingId)).ReturnsAsync((AvenSuitesApi.Domain.Entities.Invoice?)null);
        _invoiceRepositoryMock.Setup(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Invoice>())).ReturnsAsync(invoice);
        _hotelRepositoryMock.Setup(x => x.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _emailTemplateServiceMock.Setup(x => x.GenerateHotelInvoiceNotificationEmail(
            It.IsAny<string>(),      // hotelName
            It.IsAny<string?>(),      // nfseNumber
            It.IsAny<string?>(),      // nfseSeries
            It.IsAny<string?>(),      // verificationCode
            It.IsAny<string>(),       // bookingCode
            It.IsAny<string>(),       // guestName
            It.IsAny<decimal>(),      // totalServices
            It.IsAny<decimal>(),      // totalTaxes
            It.IsAny<decimal>(),      // totalAmount
            It.IsAny<DateTime>(),      // issueDate
            It.IsAny<List<InvoiceItemInfo>>(), // items
            It.IsAny<string?>(),      // erpProvider
            It.IsAny<string?>())).Returns("Email body"); // erpProtocol
        _emailServiceMock.Setup(x => x.SendEmailAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
            It.IsAny<IEnumerable<string>?>(), It.IsAny<IEnumerable<string>?>(), It.IsAny<bool>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _invoiceService.GenerateInvoiceAsync(bookingId);

        // Assert
        result.Should().NotBeNull();
        result!.BookingId.Should().Be(bookingId);
        result.TotalServices.Should().Be(300.00m);
        
        _invoiceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Invoice>()), Times.Once);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_WithInvalidBooking_ShouldReturnNull()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        
        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync((AvenSuitesApi.Domain.Entities.Booking?)null);

        // Act
        var result = await _invoiceService.GenerateInvoiceAsync(bookingId);

        // Assert
        result.Should().BeNull();
        _invoiceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Invoice>()), Times.Never);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_WithPendingBooking_ShouldReturnNull()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = bookingId,
            Status = "PENDING"
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);

        // Act
        var result = await _invoiceService.GenerateInvoiceAsync(bookingId);

        // Assert
        result.Should().BeNull();
        _invoiceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Invoice>()), Times.Never);
    }

    [Fact]
    public async Task GenerateInvoiceAsync_WithExistingInvoice_ShouldReturnExistingInvoice()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var invoiceId = Guid.NewGuid();
        
        var booking = new AvenSuitesApi.Domain.Entities.Booking
        {
            Id = bookingId,
            Status = "CONFIRMED"
        };

        var existingInvoice = new AvenSuitesApi.Domain.Entities.Invoice
        {
            Id = invoiceId,
            BookingId = bookingId,
            Status = "PENDING"
        };

        _bookingRepositoryMock.Setup(x => x.GetByIdWithDetailsAsync(bookingId)).ReturnsAsync(booking);
        _invoiceRepositoryMock.Setup(x => x.GetByBookingIdAsync(bookingId)).ReturnsAsync(existingInvoice);

        // Act
        var result = await _invoiceService.GenerateInvoiceAsync(bookingId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(invoiceId);
        _invoiceRepositoryMock.Verify(x => x.AddAsync(It.IsAny<AvenSuitesApi.Domain.Entities.Invoice>()), Times.Never);
    }

    [Fact]
    public async Task GetInvoiceByIdAsync_WithValidId_ShouldReturnInvoiceResponse()
    {
        // Arrange
        var invoiceId = Guid.NewGuid();
        var invoice = new AvenSuitesApi.Domain.Entities.Invoice
        {
            Id = invoiceId,
            Status = "PENDING",
            TotalServices = 300.00m,
            TotalTaxes = 0m,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = new List<InvoiceItem>()
        };

        _invoiceRepositoryMock.Setup(x => x.GetByIdAsync(invoiceId)).ReturnsAsync(invoice);

        // Act
        var result = await _invoiceService.GetInvoiceByIdAsync(invoiceId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(invoiceId);
    }

    [Fact]
    public async Task GetInvoicesByBookingIdAsync_WithValidBookingId_ShouldReturnInvoices()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var invoices = new List<AvenSuitesApi.Domain.Entities.Invoice>
        {
            new() { Id = Guid.NewGuid(), BookingId = bookingId, Status = "PENDING" }
        };

        _invoiceRepositoryMock.Setup(x => x.GetByBookingIdAsync(bookingId)).ReturnsAsync(invoices.First());

        // Act
        var result = await _invoiceService.GetInvoiceByBookingIdAsync(bookingId);

        // Assert
        result.Should().NotBeNull();
    }
}

