using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Application.Services.Implementations.Auth;
using AvenSuitesApi.Application.Services.Implementations.Booking;
using AvenSuitesApi.Application.Services.Implementations.Room;
using AvenSuitesApi.Application.Services.Implementations.Guest;
using AvenSuitesApi.Application.Services.Implementations.Hotel;
using AvenSuitesApi.Application.Services.Implementations.Invoice;
using AvenSuitesApi.Application.Services.Implementations;
using AvenSuitesApi.Application.Services;
using IpmNfse = AvenSuitesApi.Application.Services.Implementations.Invoice;

namespace AvenSuitesApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration? configuration = null)
    {
        // Auth Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        // Business Services
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<IGuestService, GuestService>();
        services.AddScoped<IGuestRegistrationService, GuestRegistrationService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IIpmNfseService, IpmNfseService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        
        // IPM Services (com criptografia de credenciais)
        services.AddScoped<ISecureEncryptionService, SecureEncryptionService>();
        services.AddScoped<IIpmCredentialsService, IpmCredentialsService>();
        
        // IPM HTTP Client
        services.AddHttpClient<IIpmHttpClient, IpmHttpClient>();
        
        // Current User Service
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // Email Service
        if (configuration != null)
        {
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
        }
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        
        return services;
    }
}
