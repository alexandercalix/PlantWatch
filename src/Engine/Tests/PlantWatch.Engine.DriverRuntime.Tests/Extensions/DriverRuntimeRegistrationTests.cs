using System;
using Microsoft.Extensions.DependencyInjection;
using PlantWatch.Engine.Core.Models.Definitions;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.DriverRuntime.Repositories;
using PlantWatch.DriverRuntime.Extensions;
namespace PlantWatch.DriverRuntime.Tests.Extensions;

public class DriverRuntimeRegistrationTests
{
    [Fact]
    public async Task AddDriverRuntime_ShouldInitializeAllDependencies_AndLoadDrivers()
    {
        // Arrange
        var dbPath = Path.Combine(Path.GetTempPath(), $"PlantWatch_Test_{Guid.NewGuid()}.db");
        var password = "test-password";

        // 🔧 Primero preparamos datos de prueba en la DB real
        var repo = new LiteDbConfigurationRepository(dbPath, password);

        var plc = new PlcConnectionDefinition
        {
            Id = Guid.NewGuid(),
            Name = "PLC_Test",
            DriverType = "Siemens",
            IpAddress = "192.168.0.1",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>
            {
                new PlcTagDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag1",
                    Datatype = "Bool",
                    Address = "DB1.DBX0.0"
                }
            }
        };

        await repo.SavePlcConfigurationAsync(plc);

        // ✅ Ahora simulamos el arranque de la app completa:
        var services = new ServiceCollection();

        services.AddDriverRuntime(options =>
        {
            options.LiteDbPath = dbPath;
            options.LiteDbPassword = password;
        });

        // 🔥 También registramos manualmente el Siemens DriverFactory para el test:
        var sp = services.BuildServiceProvider();

        // Act
        var manager = sp.GetRequiredService<IDriverManager>();
        var orchestrator = sp.GetRequiredService<IDriverOrchestrator>();

        // Assert
        Assert.NotNull(manager);
        Assert.NotNull(orchestrator);

        var drivers = manager.GetAllDrivers().ToList();
        Assert.Single(drivers);
        Assert.Equal("PLC_Test", drivers[0].Name);

        // Clean up DB
        if (File.Exists(dbPath))
            File.Delete(dbPath);
    }
}