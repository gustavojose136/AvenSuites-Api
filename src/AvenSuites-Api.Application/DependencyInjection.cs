using Microsoft.Extensions.DependencyInjection;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Application.Services.Implementations.Auth;

namespace AvenSuitesApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
