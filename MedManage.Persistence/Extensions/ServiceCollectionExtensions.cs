using MedManage.Persistence.Caching;
using MedManage.Persistence.Data;
using MedManage.Persistence.Migrations;
using MedManage.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AppDbContext>());

        services.AddSingleton<IInMemoryCache, InMemoryCache>();

        services.AddProxiedRepositories();
        services.AddScoped<MigrationService>();
        services.AddScoped<IDataSeeder, AdminUserSeeder>();

        return services;
    }
}
