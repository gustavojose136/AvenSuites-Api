using Microsoft.Extensions.DependencyInjection;
using AvenSuitesApi.Application.Services.Interfaces;
using AvenSuitesApi.Application.Services.Implementations.Auth;
using AvenSuitesApi.Application.Services.Implementations.Booking;
using AvenSuitesApi.Application.Services.Implementations.Room;
using AvenSuitesApi.Application.Services.Implementations.Guest;
using AvenSuitesApi.Application.Services.Implementations.Hotel;
using AvenSuitesApi.Application.Services.Implementations.Invoice;
using IpmNfse = AvenSuitesApi.Application.Services.Implementations.Invoice;

namespace AvenSuitesApi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Auth Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        // Business Services
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IRoomTypeService, RoomTypeService>();
        services.AddScoped<IGuestService, GuestService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<IIpmNfseService, IpmNfseService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        
        // IPM HTTP Client
        services.AddHttpClient<IIpmHttpClient, IpmHttpClient>();
        
        return services;
    }
}
