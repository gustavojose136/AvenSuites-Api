using AvenSuitesApi.Domain.Entities;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AvenSuitesApi.Middleware;

public class AuditMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditMiddleware> _logger;

    public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        // Processar a requisição
        await _next(context);

        // Registrar auditoria apenas se autenticado e for método mutação
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var method = context.Request.Method;
            if (IsMutationMethod(method))
            {
                try
                {
                    var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                     context.User.FindFirst("sub")?.Value;

                    if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
                    {
                        var hotelIdClaim = context.User.FindFirst("hotel_id")?.Value;
                        Guid? hotelId = null;
                        if (!string.IsNullOrEmpty(hotelIdClaim) && Guid.TryParse(hotelIdClaim, out var hotelGuid))
                        {
                            hotelId = hotelGuid;
                        }

                        var entityName = context.Request.RouteValues["controller"]?.ToString() ?? "Unknown";
                        var entityId = context.Request.RouteValues["id"]?.ToString() ?? "Unknown";

                        var auditLog = new AuditLog
                        {
                            Id = Guid.NewGuid(),
                            HotelId = hotelId,
                            ActorUserId = userId,
                            EntityName = entityName,
                            EntityId = Guid.TryParse(entityId, out var entityGuid) ? entityGuid : Guid.NewGuid(),
                            Action = MapHttpMethodToAction(method),
                            ChangesJson = await GetRequestBodyAsync(context),
                            CreatedAt = DateTime.UtcNow
                        };

                        dbContext.AuditLogs.Add(auditLog);
                        await dbContext.SaveChangesAsync();

                        _logger.LogInformation(
                            "Audit log criado: {Action} em {EntityName} por usuário {UserId}",
                            method, entityName, userId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar audit log");
                    // Não bloquear a requisição se falhar o audit
                }
            }
        }
    }

    private static bool IsMutationMethod(string method)
    {
        return method == "POST" || method == "PUT" || method == "PATCH" || method == "DELETE";
    }

    private static string MapHttpMethodToAction(string method)
    {
        return method switch
        {
            "POST" => "INSERT",
            "PUT" => "UPDATE",
            "PATCH" => "UPDATE",
            "DELETE" => "DELETE",
            _ => "UNKNOWN"
        };
    }

    private async Task<string?> GetRequestBodyAsync(HttpContext context)
    {
        try
        {
            if (context.Request.Body.CanSeek)
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
                return body.Length > 500 ? body[..500] : body;
            }
        }
        catch
        {
            // Ignorar erros
        }

        return null;
    }
}

