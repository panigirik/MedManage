using Castle.DynamicProxy;
using MedManage.Domain.Interfaces;
using MedManage.Persistence.Repositories;
using MedManage.Persistence.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace MedManage.Persistence.Extensions;

internal static class RepositoryRegistrationExtensions
{
    public static IServiceCollection AddTransactionalRepositories(this IServiceCollection services)
    {
        services.AddSingleton<ProxyGenerator>();
        services.AddScoped<TransactionInterceptor>();

        services.AddScoped<IUserRepository>(provider =>
            CreateTransactionalProxy<IUserRepository, UserRepository>(provider));

        services.AddScoped<IAnnouncementRepository>(provider =>
            CreateTransactionalProxy<IAnnouncementRepository, AnnouncementRepository>(provider));

        return services;
    }

    private static TInterface CreateTransactionalProxy<TInterface, TImplementation>(IServiceProvider provider)
    where TInterface : class
    where TImplementation : class, TInterface
    {
        var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
        var target = ActivatorUtilities.CreateInstance<TImplementation>(provider);
        var interceptor = provider.GetRequiredService<TransactionInterceptor>();

        return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(target, interceptor);
    }
}
