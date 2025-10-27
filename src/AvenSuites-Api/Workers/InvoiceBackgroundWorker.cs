using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AvenSuitesApi.Workers;

public class InvoiceBackgroundWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InvoiceBackgroundWorker> _logger;

    public InvoiceBackgroundWorker(
        IServiceProvider serviceProvider,
        ILogger<InvoiceBackgroundWorker> logger)
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
                var invoiceRepository = scope.ServiceProvider.GetRequiredService<IInvoiceRepository>();

                // Buscar invoices com status PENDING
                var pendingInvoices = await invoiceRepository.GetByStatusAsync("PENDING");

                foreach (var invoice in pendingInvoices)
                {
                    try
                    {
                        _logger.LogInformation("Processando invoice {InvoiceId} para booking {BookingId}",
                            invoice.Id, invoice.BookingId);

                        // Atualizar status para PROCESSING
                        invoice.Status = "PROCESSING";
                        invoice.UpdatedAt = DateTime.UtcNow;
                        await invoiceRepository.UpdateAsync(invoice);

                        // Aqui você pode adicionar a lógica para chamar IpmNfseService
                        // var ipmService = scope.ServiceProvider.GetRequiredService<IIpmNfseService>();
                        // await ipmService.GenerateInvoiceAsync(...);

                        _logger.LogInformation("Invoice {InvoiceId} processado com sucesso", invoice.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Erro ao processar invoice {InvoiceId}", invoice.Id);
                        
                        invoice.Status = "FAILED";
                        invoice.UpdatedAt = DateTime.UtcNow;
                        await invoiceRepository.UpdateAsync(invoice);
                    }
                }

                // Aguardar 5 minutos antes de executar novamente
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no InvoiceBackgroundWorker");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}

