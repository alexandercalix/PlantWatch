using System;
using Microsoft.Extensions.DependencyInjection;
using PlantWatch.Core.Interfaces;
using PlantWatch.DriverRuntime.Configurations;
using PlantWatch.DriverRuntime.Repositories;

namespace PlantWatch.DriverRuntime.Extensions;

public static class DriverRuntimeServiceCollectionExtensions
{
    public static IServiceCollection AddDriverRuntime(this IServiceCollection services, Action<DriverRuntimeOptions> configure)
    {
        // Bind config options
        var options = new DriverRuntimeOptions();
        configure(options);
        services.AddSingleton(options);

        // Register repository
        services.AddSingleton<IConfigurationRepository>(sp =>
            new LiteDbConfigurationRepository(options.LiteDbPath, options.LiteDbPassword));

        // Register DriverManager
        services.AddSingleton<DriverManager>();

        // 🔥 Hook para inicializar DriverManager al arranque
        services.AddHostedService<DriverManagerInitializer>();

        return services;
    }
}