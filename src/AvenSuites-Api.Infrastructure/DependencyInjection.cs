using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;
using Pomelo.EntityFrameworkCore.MySql;

namespace AvenSuitesApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(
                configuration.GetConnectionString("DefaultConnection"),
                new MySqlServerVersion(new Version(8, 0, 0)),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IGuestRepository, GuestRepository>();
        services.AddScoped<IGuestPiiRepository, GuestPiiRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
        services.AddScoped<IAmenityRepository, AmenityRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingRoomRepository, BookingRoomRepository>();
        services.AddScoped<IRatePlanRepository, RatePlanRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IMaintenanceBlockRepository, MaintenanceBlockRepository>();
        services.AddScoped<IIpmCredentialsRepository, IpmCredentialsRepository>();
        services.AddScoped<IErpIntegrationLogRepository, ErpIntegrationLogRepository>();

        return services;
    }
}
