using AvenSuitesApi.Application.DTOs.Invoice;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using BookingRoomInfo = AvenSuitesApi.Application.Services.Interfaces.BookingRoomInfo;
using InvoiceItemInfo = AvenSuitesApi.Application.Services.Interfaces.InvoiceItemInfo;

namespace AvenSuitesApi.Application.Services.Implementations.Invoice;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IBookingRepository bookingRepository,
        IHotelRepository hotelRepository,
        IEmailService emailService,
        IEmailTemplateService emailTemplateService,
        ILogger<InvoiceService> logger)
    {
        _invoiceRepository = invoiceRepository;
        _bookingRepository = bookingRepository;
        _hotelRepository = hotelRepository;
        _emailService = emailService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task<InvoiceResponse?> GenerateInvoiceAsync(Guid bookingId)
    {
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(bookingId);
        if (booking == null || booking.Status != "CONFIRMED")
            return null;

        var existingInvoice = await _invoiceRepository.GetByBookingIdAsync(bookingId);
        if (existingInvoice != null)
            return MapToResponse(existingInvoice);

        var totalServices = booking.BookingRooms.Sum(br => br.PriceTotal);
        var totalTaxes = 0m;

        var invoice = new Domain.Entities.Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            HotelId = booking.HotelId,
            Status = "PENDING",
            TotalServices = totalServices,
            TotalTaxes = totalTaxes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = booking.BookingRooms.Select(br => new InvoiceItem
            {
                Id = Guid.NewGuid(),
                InvoiceId = Guid.Empty,
                Description = $"Diária - Quarto {br.Room.RoomNumber}",
                Quantity = (booking.CheckOutDate - booking.CheckInDate).Days,
                UnitPrice = br.PriceTotal / (booking.CheckOutDate - booking.CheckInDate).Days,
                Total = br.PriceTotal,
                TaxCode = "901",
                TaxRate = 2.01m
            }).ToList()
        };

        var createdInvoice = await _invoiceRepository.AddAsync(invoice);
        var invoiceResponse = MapToResponse(createdInvoice);

        // Enviar e-mail de notificação para o hotel
        try
        {
            var hotel = await _hotelRepository.GetByIdAsync(booking.HotelId);
            if (hotel != null && !string.IsNullOrWhiteSpace(hotel.Email))
            {
                var guestPii = booking.MainGuest?.GuestPii;
                var guestName = guestPii?.FullName ?? "Hóspede";
                var bookingCode = booking.Code;

                var items = createdInvoice.Items.Select(i => new InvoiceItemInfo
                {
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.Total,
                    TaxCode = i.TaxCode,
                    TaxRate = i.TaxRate
                }).ToList();

                var totalAmount = createdInvoice.TotalServices + createdInvoice.TotalTaxes;
                var issueDate = createdInvoice.IssueDate ?? DateTime.UtcNow;

                var emailBody = _emailTemplateService.GenerateHotelInvoiceNotificationEmail(
                    hotelName: hotel.Name,
                    nfseNumber: createdInvoice.NfseNumber,
                    nfseSeries: createdInvoice.NfseSeries,
                    verificationCode: createdInvoice.VerificationCode,
                    bookingCode: bookingCode,
                    guestName: guestName,
                    totalServices: createdInvoice.TotalServices,
                    totalTaxes: createdInvoice.TotalTaxes,
                    totalAmount: totalAmount,
                    issueDate: issueDate,
                    items: items,
                    erpProvider: createdInvoice.ErpProvider,
                    erpProtocol: createdInvoice.ErpProtocol);

                await _emailService.SendEmailAsync(
                    to: hotel.Email,
                    subject: $"Nota Fiscal Gerada - {createdInvoice.NfseNumber ?? "N/A"}",
                    body: emailBody,
                    isHtml: true,
                    cc: null,
                    bcc: null);

                _logger.LogInformation("E-mail de notificação de nota fiscal enviado para hotel {HotelEmail}, Invoice: {InvoiceId}",
                    hotel.Email, createdInvoice.Id);
            }
        }
        catch (Exception emailEx)
        {
            // Não falhar a criação da nota se o e-mail falhar
            _logger.LogWarning(emailEx, "Erro ao enviar e-mail de notificação de nota fiscal para hotel, Invoice: {InvoiceId}",
                createdInvoice.Id);
        }

        return invoiceResponse;
    }

    public async Task<InvoiceResponse?> GenerateInvoiceFromBookingAsync(Guid bookingId)
    {
        return await GenerateInvoiceAsync(bookingId);
    }

    public async Task<InvoiceResponse?> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdWithItemsAsync(id);
        if (invoice == null)
            return null;

        return MapToResponse(invoice);
    }

    public async Task<InvoiceResponse?> GetInvoiceByBookingIdAsync(Guid bookingId)
    {
        var invoice = await _invoiceRepository.GetByBookingIdAsync(bookingId);
        if (invoice == null)
            return null;

        return MapToResponse(invoice);
    }

    public async Task<IEnumerable<InvoiceResponse>> GetInvoicesByHotelAsync(Guid hotelId)
    {
        var invoices = await _invoiceRepository.GetByHotelIdAsync(hotelId);
        return invoices.Select(MapToResponse);
    }

    public async Task<bool> EmitInvoiceToErpAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
        if (invoice == null || invoice.Status != "PENDING")
            return false;

        // Implementar chamada ao ERP/IPM
        invoice.Status = "PROCESSING";
        await _invoiceRepository.UpdateAsync(invoice);

        return true;
    }

    public async Task<InvoiceResponse?> CancelInvoiceAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
        if (invoice == null || invoice.Status == "CANCELLED")
            return null;

        invoice.Status = "CANCELLED";
        invoice.UpdatedAt = DateTime.UtcNow;
        
        var updatedInvoice = await _invoiceRepository.UpdateAsync(invoice);
        return MapToResponse(updatedInvoice);
    }

    private static InvoiceResponse MapToResponse(Domain.Entities.Invoice invoice)
    {
        return new InvoiceResponse
        {
            Id = invoice.Id,
            BookingId = invoice.BookingId,
            HotelId = invoice.HotelId,
            Status = invoice.Status,
            IssueDate = invoice.IssueDate,
            NfseNumber = invoice.NfseNumber,
            NfseSeries = invoice.NfseSeries,
            RpsNumber = invoice.RpsNumber,
            VerificationCode = invoice.VerificationCode,
            ErpProvider = invoice.ErpProvider,
            ErpProtocol = invoice.ErpProtocol,
            TotalServices = invoice.TotalServices,
            TotalTaxes = invoice.TotalTaxes,
            XmlS3Key = invoice.XmlS3Key,
            PdfS3Key = invoice.PdfS3Key,
            CreatedAt = invoice.CreatedAt,
            UpdatedAt = invoice.UpdatedAt,
            Items = invoice.Items.Select(i => new InvoiceItemResponse
            {
                Id = i.Id,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TaxCode = i.TaxCode,
                TaxRate = i.TaxRate,
                Total = i.Total
            }).ToList()
        };
    }
}

