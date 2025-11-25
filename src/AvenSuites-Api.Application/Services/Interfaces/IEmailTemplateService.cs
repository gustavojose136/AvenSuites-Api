namespace AvenSuitesApi.Application.Services.Interfaces;

/// <summary>
/// Interface para serviço de templates de e-mail
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Gera o template HTML de boas-vindas para novo hóspede
    /// </summary>
    string GenerateWelcomeEmail(string guestName, string hotelName);

    /// <summary>
    /// Gera o template HTML de confirmação de reserva (ticket)
    /// </summary>
    string GenerateBookingConfirmationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        decimal totalAmount,
        string currency,
        List<BookingRoomInfo> rooms,
        string? hotelAddress = null,
        string? hotelPhone = null);

    /// <summary>
    /// Gera o template HTML de lembrete de reserva (3 dias antes)
    /// </summary>
    string GenerateBookingReminderEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        List<BookingRoomInfo> rooms,
        string? hotelAddress = null,
        string? hotelPhone = null);

    /// <summary>
    /// Gera o template HTML de notificação de nova reserva para o hotel
    /// </summary>
    string GenerateHotelBookingNotificationEmail(
        string hotelName,
        string bookingCode,
        string guestName,
        string guestEmail,
        string? guestPhone,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        int adults,
        int children,
        decimal totalAmount,
        string currency,
        List<BookingRoomInfo> rooms,
        string? notes = null,
        string? channelRef = null);

    /// <summary>
    /// Gera o template HTML de notificação de nota fiscal gerada para o hotel
    /// </summary>
    string GenerateHotelInvoiceNotificationEmail(
        string hotelName,
        string? nfseNumber,
        string? nfseSeries,
        string? verificationCode,
        string? bookingCode,
        string guestName,
        decimal totalServices,
        decimal totalTaxes,
        decimal totalAmount,
        DateTime issueDate,
        List<InvoiceItemInfo> items,
        string? erpProvider = null,
        string? erpProtocol = null);

    /// <summary>
    /// Gera o template HTML de cancelamento de reserva para o hóspede
    /// </summary>
    string GenerateBookingCancellationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal totalAmount,
        string currency,
        string? reason = null);

    /// <summary>
    /// Gera o template HTML de notificação de cancelamento de reserva para o hotel
    /// </summary>
    string GenerateHotelBookingCancellationEmail(
        string hotelName,
        string bookingCode,
        string guestName,
        string guestEmail,
        DateTime checkInDate,
        DateTime checkOutDate,
        decimal totalAmount,
        string currency,
        string? reason = null);

    /// <summary>
    /// Gera o template HTML de confirmação de check-in para o hóspede
    /// </summary>
    string GenerateCheckInConfirmationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        List<BookingRoomInfo> rooms,
        DateTime checkOutDate,
        string? hotelAddress = null,
        string? hotelPhone = null);

    /// <summary>
    /// Gera o template HTML de confirmação de check-out para o hóspede
    /// </summary>
    string GenerateCheckOutConfirmationEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        decimal totalAmount,
        string currency,
        string? hotelAddress = null,
        string? hotelPhone = null);

    /// <summary>
    /// Gera o template HTML de confirmação de reserva (quando hotel confirma)
    /// </summary>
    string GenerateBookingConfirmedEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        List<BookingRoomInfo> rooms,
        string? hotelAddress = null,
        string? hotelPhone = null);

    /// <summary>
    /// Gera o template HTML de lembrete de check-out (1 dia antes)
    /// </summary>
    string GenerateCheckOutReminderEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkOutDate,
        string? hotelAddress = null,
        string? hotelPhone = null);

    /// <summary>
    /// Gera o template HTML de agradecimento pós-estadia
    /// </summary>
    string GeneratePostStayThankYouEmail(
        string guestName,
        string hotelName,
        string bookingCode,
        DateTime checkInDate,
        DateTime checkOutDate,
        int nights,
        string? hotelAddress = null,
        string? hotelPhone = null);
}

/// <summary>
/// Informações do item da nota fiscal para o template de e-mail
/// </summary>
public class InvoiceItemInfo
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string? TaxCode { get; set; }
    public decimal? TaxRate { get; set; }
}

/// <summary>
/// Informações do quarto para o template de e-mail
/// </summary>
public class BookingRoomInfo
{
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomTypeName { get; set; } = string.Empty;
    public decimal PriceTotal { get; set; }
}

