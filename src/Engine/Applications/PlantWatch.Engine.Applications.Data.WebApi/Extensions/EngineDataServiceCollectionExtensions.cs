using System;
using Microsoft.Extensions.DependencyInjection;
using PlantWatch.Engine.Applications.Data.WebApi.Controllers;
using PlantWatch.Engine.Core.Common;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Data;
using PlantWatch.Engine.Data.Drivers.MsSql;
using PlantWatch.Engine.Data.Managers;
using PlantWatch.Engine.Data.Repositories;
using PlantWatch.Engine.Data.Services;

namespace PlantWatch.Engine.Applications.Data.WebApi;

public static class EngineDataServiceCollectionExtensions
{
    public static IServiceCollection AddEngineData(this IServiceCollection services, Action<StorageConfigurationOptions> configure)
    {
        var options = new StorageConfigurationOptions();
        configure(options);
        services.AddSingleton(options);

        // LiteDB Repository
        services.AddSingleton<IDatabaseConfigurationRepository>(
            sp => new LiteDbDatabaseConfigurationRepository(options.LiteDbPath, options.LiteDbPassword));

        // Driver Manager
        services.AddSingleton<IDatabaseDriverManager>(sp =>
        {
            var repo = sp.GetRequiredService<IDatabaseConfigurationRepository>();
            var manager = new DatabaseDriverManager(repo);

            // Register driver factories here (self-contained)
            manager.RegisterDriverFactory(new MsSqlDriverFactory());

            manager.ReloadDriversAsync().GetAwaiter().GetResult();

            return manager;
        });

        services.AddSingleton<IDatabaseDriverOrchestrator, DatabaseDriverOrchestrator>();

        return services;
    }

    public static IServiceCollection AddEngineDataWebApi(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(DiagnosticsController).Assembly)
            .AddApplicationPart(typeof(CommandsController).Assembly)
            .AddApplicationPart(typeof(DriversController).Assembly)
            .AddApplicationPart(typeof(DatabaseConfigurationController).Assembly);

        return services;
    }
}

