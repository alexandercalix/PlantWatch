
using Microsoft.Extensions.DependencyInjection;
using PlantWatch.DriverRuntime;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.DriverRuntime.Repositories;
using PlantWatch.Engine.Core.Factories;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.DriverRuntime.Configurations;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Factories;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Validators;

namespace PlantWatch.Engine.Applications.WebApi;

public static class EngineDriverServiceCollectionExtensions
{
    public static IServiceCollection AddEngineDrivers(this IServiceCollection services, Action<DriverRuntimeOptions> configure)
    {
        // TODO: aquí movemos todo lo que ya tenías en AddDriverRuntime()

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

            // Registramos los factories y validators (los de Siemens, etc.)
            manager.RegisterDriverFactory((IDriverFactory)new SiemensPLCServiceFactory(), new SiemensConfigurationValidator());

            // 🔥 Cargamos los drivers al arrancar
            manager.ReloadDriversAsync().GetAwaiter().GetResult();

            return manager;
        });

        // Si el día de mañana hay más factories, se registran aquí

        services.AddHostedService<DriverManagerInitializer>();

        return services;
    }


}