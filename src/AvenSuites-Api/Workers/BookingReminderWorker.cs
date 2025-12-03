using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BookingRoomInfo = AvenSuitesApi.Application.Services.Interfaces.BookingRoomInfo;

namespace AvenSuitesApi.Workers;

public class BookingReminderWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BookingReminderWorker> _logger;

    public BookingReminderWorker(
        IServiceProvider serviceProvider,
        ILogger<BookingReminderWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var emailTemplateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

                var targetCheckInDate = DateTime.UtcNow.Date.AddDays(3);
                var targetCheckInDateEnd = targetCheckInDate.AddDays(1);

                var bookingsToRemind = await context.Bookings
                    .Include(b => b.MainGuest)
                        .ThenInclude(g => g.GuestPii)
                    .Include(b => b.Hotel)
                    .Include(b => b.BookingRooms)
                        .ThenInclude(br => br.Room)
                    .Include(b => b.BookingRooms)
                        .ThenInclude(br => br.RoomType)
                    .AsSplitQuery()
                    .Where(b => b.CheckInDate.Date >= targetCheckInDate
                        && b.CheckInDate.Date < targetCheckInDateEnd
                        && (b.Status == "CONFIRMED" || b.Status == "PENDING")
                        && b.MainGuest != null
                        && b.MainGuest.GuestPii != null
                        && !string.IsNullOrWhiteSpace(b.MainGuest.GuestPii.Email))
                    .ToListAsync();

                _logger.LogInformation("Encontradas {Count} reservas para enviar lembrete", bookingsToRemind.Count);

                foreach (var booking in bookingsToRemind)
                {
                    try
                    {
                        var guestPii = booking.MainGuest?.GuestPii;
                        if (guestPii == null)
                            continue;

                        var guestEmail = guestPii.Email;
                        var guestName = guestPii.FullName ?? "Hóspede";
                        var hotel = booking.Hotel;
                        var hotelName = hotel?.Name ?? "Hotel";

                        if (string.IsNullOrWhiteSpace(guestEmail))
                            continue;

                        var nights = (booking.CheckOutDate - booking.CheckInDate).Days;
                        var rooms = booking.BookingRooms
                            .Where(br => br.Room != null && br.RoomType != null)
                            .GroupBy(br => br.RoomId)
                            .Select(g => g.First())
                            .Select(br => new BookingRoomInfo
                            {
                                RoomNumber = br.Room!.RoomNumber,
                                RoomTypeName = br.RoomType!.Name ?? "",
                                PriceTotal = br.PriceTotal
                            })
                            .ToList();

                        if (rooms.Count == 0)
                            continue;

                        var hotelAddress = hotel != null && !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                            ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                            : null;

                        var emailBody = emailTemplateService.GenerateBookingReminderEmail(
                            guestName: guestName,
                            hotelName: hotelName,
                            bookingCode: booking.Code,
                            checkInDate: booking.CheckInDate,
                            checkOutDate: booking.CheckOutDate,
                            nights: nights,
                            rooms: rooms,
                            hotelAddress: hotelAddress,
                            hotelPhone: hotel?.PhoneE164);

                        var emailSent = await emailService.SendEmailAsync(
                            to: guestEmail,
                            subject: $"Lembrete: Sua reserva no {hotelName} está chegando!",
                            body: emailBody,
                            isHtml: true,
                            cc: null,
                            bcc: null);

                        if (emailSent)
                        {
                            _logger.LogInformation(
                                "Lembrete de reserva enviado com sucesso para {Email}, Booking: {BookingCode}",
                                guestEmail, booking.Code);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Falha ao enviar lembrete de reserva para {Email}, Booking: {BookingCode}",
                                guestEmail, booking.Code);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Erro ao enviar lembrete de reserva para Booking: {BookingId}",
                            booking.Id);
                    }
                }

                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1).AddHours(9);
                if (nextRun <= now)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                _logger.LogInformation("Próxima execução do BookingReminderWorker em {Delay} horas", delay.TotalHours);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no BookingReminderWorker");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}

