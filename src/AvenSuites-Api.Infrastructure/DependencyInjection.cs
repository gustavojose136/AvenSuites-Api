using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AvenSuitesApi.Domain.Interfaces;
using AvenSuitesApi.Infrastructure.Data.Contexts;
using AvenSuitesApi.Infrastructure.Repositories.Implementations;

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

        return services;
    }
}
