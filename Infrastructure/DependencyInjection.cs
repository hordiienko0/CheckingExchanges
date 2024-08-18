using Domain.Interfaces;
using Infrastructure.External;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {

        services.AddTransient<IExchangeApiClient, BinanceApiClient>();
        services.AddTransient<IExchangeApiClient, KuCoinApiClient>();
        return services;
    }
}