using System;
using Microsoft.Extensions.DependencyInjection;
using PlantWatch.Engine.Applications.Data.WebApi.Controllers;
using PlantWatch.Engine.Data.Managers;
using PlantWatch.Engine.Data.Services;

namespace PlantWatch.Engine.Applications.Data.WebApi.Extensions;

public static class EngineDataServiceCollectionExtensions
{
    public static IServiceCollection AddEngineData(this IServiceCollection services)
    {
        // Load and register all database drivers
        var driverManager = new DatabaseDriverManager();
        var drivers = DriverLoader.LoadDriversFromJson(ConfigurationPaths.DriversJsonPath);

        foreach (var driver in drivers)
            driverManager.RegisterDriver(driver);

        services.AddSingleton(driverManager);

        // Add the API controllers from this assembly
        services.AddControllers()
            .AddApplicationPart(typeof(DiagnosticsController).Assembly)
            .AddApplicationPart(typeof(SchemaController).Assembly)
            .AddApplicationPart(typeof(CommandsController).Assembly);

        return services;
    }


}