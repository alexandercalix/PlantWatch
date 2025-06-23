using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlantWatch.Engine.Applications.WebApi;
using PlantWatch.Engine.DriverRuntime.Configurations;
using PlantWatch.Engine.DriverRuntime;
using PlantWatch.DriverRuntime;


var builder = WebApplication.CreateBuilder(args);


builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(PlantWatch.Engine.Applications.WebApi.Controllers.DiagnosticsController).Assembly)
    .AddApplicationPart(typeof(PlantWatch.Engine.Applications.WebApi.Controllers.PlcConfigurationController).Assembly)
    .AddApplicationPart(typeof(PlantWatch.Engine.Applications.WebApi.Controllers.TagsController).Assembly);

// Aquí levantamos el runtime completo
builder.Services.AddEngineDrivers(options =>
{
    options.LiteDbPath = "test_runtime.db";  // O el path que quieras
    options.LiteDbPassword = "super-secret";
});

// Y aquí agregamos el API Web
builder.Services.AddEngineWebApi();  // Esta es la extensión que preparamos en la librería WebApi

var app = builder.Build();

app.MapControllers();

foreach (var controller in builder.Services.BuildServiceProvider()
             .GetRequiredService<Microsoft.AspNetCore.Mvc.Infrastructure.IActionDescriptorCollectionProvider>()
             .ActionDescriptors.Items)
{
    Console.WriteLine($"[MVC] Controller loaded: {controller.DisplayName}");
}

app.Run();
