using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvenSuitesApi.Workers;

public class IntegrationEventPublisher : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<IntegrationEventPublisher> _logger;

    public IntegrationEventPublisher(
        IServiceProvider serviceProvider,
        ILogger<IntegrationEventPublisher> logger)
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

                // Buscar eventos pendentes
                var pendingEvents = await context.IntegrationEventOutbox
                    .Where(e => e.Status == "PENDING")
                    .OrderBy(e => e.CreatedAt)
                    .Take(100)
                    .ToListAsync(stoppingToken);

                foreach (var @event in pendingEvents)
                {
                    try
                    {
                        _logger.LogInformation(
                            "Publicando evento {EventType} para aggregate {AggregateType}:{AggregateId}",
                            @event.EventType, @event.AggregateType, @event.AggregateId);

                        // TODO: Aqui você publicaria no RabbitMQ
                        // await _rabbitMqPublisher.PublishAsync(@event);

                        // Por enquanto, apenas marcar como publicado
                        @event.Status = "PUBLISHED";
                        @event.PublishedAt = DateTime.UtcNow;
                        @event.Attempts++;

                        await context.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation(
                            "Evento {EventId} publicado com sucesso", @event.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao publicar evento {EventId}", @event.Id);

                        @event.Attempts++;
                        @event.LastError = ex.Message;

                        // Se tentou mais de 5 vezes, marcar como falho
                        if (@event.Attempts >= 5)
                        {
                            @event.Status = "FAILED";
                        }

                        await context.SaveChangesAsync(stoppingToken);
                    }
                }

                // Aguardar 10 segundos antes de executar novamente
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no IntegrationEventPublisher");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}

