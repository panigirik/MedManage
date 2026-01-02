using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MedManage.Domain.Interfaces;
using MedManage.Persistence.Data;
using MedManage.Persistence.Repositories;

namespace UserManagement.Persistence.Extensions
{
    public static class ServiceCollectionExtensio
    {
        public static void AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрация контекстов для базы данных
            services.AddDbContext<UserDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(UserDbContext).Assembly.FullName)));

            services.AddDbContext<AnnouncementDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(AnnouncementDbContext).Assembly.FullName)));

            services.AddDbContext<ProductDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ProductDbContext).Assembly.FullName)));
        }

        // Регистрация сервисов
        public static void AddInfrastructureRepositoriesServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Регистрация репозиториев
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
        }
    }
}