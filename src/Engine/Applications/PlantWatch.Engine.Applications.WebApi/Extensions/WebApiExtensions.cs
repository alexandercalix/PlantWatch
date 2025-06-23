using System;
using Microsoft.Extensions.DependencyInjection;

namespace PlantWatch.Engine.Applications.WebApi;

public static class WebApiExtensions
{
    public static IServiceCollection AddEngineWebApi(this IServiceCollection services)
    {
        services.AddControllers();
        services
    .AddControllers()
    .AddApplicationPart(typeof(PlantWatch.Engine.Applications.WebApi.Controllers.DiagnosticsController).Assembly)
    .AddApplicationPart(typeof(PlantWatch.Engine.Applications.WebApi.Controllers.PlcConfigurationController).Assembly)
    .AddApplicationPart(typeof(PlantWatch.Engine.Applications.WebApi.Controllers.TagsController).Assembly);
        return services;
    }
}