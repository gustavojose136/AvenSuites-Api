using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvenSuitesApi.Workers;

/// <summary>
/// Worker que envia lembretes de check-out 1 dia antes
/// </summary>
public class CheckOutReminderWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CheckOutReminderWorker> _logger;

    public CheckOutReminderWorker(
        IServiceProvider serviceProvider,
        ILogger<CheckOutReminderWorker> logger)
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

                // Data de check-out 1 dia a partir de hoje
                var targetCheckOutDate = DateTime.UtcNow.Date.AddDays(1);
                var targetCheckOutDateEnd = targetCheckOutDate.AddDays(1);

                // Buscar reservas com check-out em 1 dia e status CHECKED_IN
                var bookingsToRemind = await context.Bookings
                    .Include(b => b.MainGuest)
                        .ThenInclude(g => g.GuestPii)
                    .Include(b => b.Hotel)
                    .Where(b => b.CheckOutDate.Date >= targetCheckOutDate
                        && b.CheckOutDate.Date < targetCheckOutDateEnd
                        && b.Status == "CHECKED_IN"
                        && b.MainGuest != null
                        && b.MainGuest.GuestPii != null
                        && !string.IsNullOrWhiteSpace(b.MainGuest.GuestPii.Email))
                    .ToListAsync();

                _logger.LogInformation("Encontradas {Count} reservas para enviar lembrete de check-out", bookingsToRemind.Count);

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

                        var hotelAddress = hotel != null && !string.IsNullOrWhiteSpace(hotel.AddressLine1)
                            ? $"{hotel.AddressLine1}{(string.IsNullOrWhiteSpace(hotel.AddressLine2) ? "" : $", {hotel.AddressLine2}")}, {hotel.City} - {hotel.State}"
                            : null;

                        var emailBody = emailTemplateService.GenerateCheckOutReminderEmail(
                            guestName: guestName,
                            hotelName: hotelName,
                            bookingCode: booking.Code,
                            checkOutDate: booking.CheckOutDate,
                            hotelAddress: hotelAddress,
                            hotelPhone: hotel?.PhoneE164);

                        var emailSent = await emailService.SendEmailAsync(
                            to: guestEmail,
                            subject: $"Lembrete: Check-out amanhã - {hotelName}",
                            body: emailBody,
                            isHtml: true,
                            cc: null,
                            bcc: null);

                        if (emailSent)
                        {
                            _logger.LogInformation(
                                "Lembrete de check-out enviado com sucesso para {Email}, Booking: {BookingCode}",
                                guestEmail, booking.Code);
                        }
                        else
                        {
                            _logger.LogWarning(
                                "Falha ao enviar lembrete de check-out para {Email}, Booking: {BookingCode}",
                                guestEmail, booking.Code);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex,
                            "Erro ao enviar lembrete de check-out para Booking: {BookingId}",
                            booking.Id);
                    }
                }

                // Executar uma vez por dia às 10:00 AM
                var now = DateTime.UtcNow;
                var nextRun = now.Date.AddDays(1).AddHours(10);
                if (nextRun <= now)
                    nextRun = nextRun.AddDays(1);

                var delay = nextRun - now;
                _logger.LogInformation("Próxima execução do CheckOutReminderWorker em {Delay} horas", delay.TotalHours);
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no CheckOutReminderWorker");
                // Em caso de erro, aguardar 1 hora antes de tentar novamente
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}


