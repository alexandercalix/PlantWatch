using System;
using Microsoft.Extensions.DependencyInjection;
using PlantWatch.Engine.Core.Factories;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.DriverRuntime.Configurations;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.DriverRuntime.Repositories;
using PlantWatch.Engine.Drivers.Siemens.Factories;
using PlantWatch.Engine.Drivers.Siemens.Validators;

namespace PlantWatch.DriverRuntime.Extensions;

public static class DriverRuntimeServiceCollectionExtensions
{
    public static IServiceCollection AddDriverRuntime(this IServiceCollection services, Action<DriverRuntimeOptions> configure)
    {
        // Bind config options
        var options = new DriverRuntimeOptions();
        configure(options);
        services.AddSingleton(options);

        // Repository (LiteDB)
        services.AddSingleton<IConfigurationRepository>(
            sp => new LiteDbConfigurationRepository(options.LiteDbPath, options.LiteDbPassword));

        // Driver Manager & Orchestrator

        services.AddSingleton<IDriverOrchestrator, DriverOrchestrator>();
        services.AddSingleton<IDriverManager>(sp =>
        {
            var repo = sp.GetRequiredService<IConfigurationRepository>();
            var manager = new DriverManager(repo);

            // Aquí registras los factories y validators...
            manager.RegisterDriverFactory((IDriverFactory)new SiemensPLCServiceFactory(), new SiemensConfigurationValidator());

            // 🔥 Cargamos los drivers al momento de registrarlo
            manager.ReloadDriversAsync().GetAwaiter().GetResult();

            return manager;
        });

        services.AddSingleton<PlantWatch.Engine.Core.Factories.IDriverFactory, SiemensPLCServiceFactory>();
        services.AddSingleton<PlantWatch.Engine.Core.Validators.IConfigurationValidator, SiemensConfigurationValidator>();

        // Inicializador en background
        services.AddHostedService<DriverManagerInitializer>();

        return services;
    }
}