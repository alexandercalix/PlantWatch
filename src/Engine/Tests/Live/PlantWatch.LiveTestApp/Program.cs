
using PlantWatch.Engine.Core.Models.Definitions;

using PlantWatch.Engine.Drivers.Protocols.Siemens;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Factories;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Validators;

using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.Core.Factories;
using PlantWatch.Engine.Core.Validators;
using PlantWatch.Engine.DriverRuntime.Configurations;
using PlantWatch.DriverRuntime.Repositories;
using PlantWatch.DriverRuntime;

// 1️⃣ Configuración manual
var options = new DriverRuntimeOptions
{
    LiteDbPath = "test_runtime.db",
    LiteDbPassword = "test-password"
};

// 2️⃣ Repositorio
var repository = new LiteDbConfigurationRepository(options.LiteDbPath, options.LiteDbPassword);

// 3️⃣ Driver Manager
var driverManager = new DriverManager(repository);

// 4️⃣ Registrar Siemens Driver Factory y Validator
var siemensFactory = new SiemensPLCServiceFactory();
var siemensValidator = new SiemensConfigurationValidator();
driverManager.RegisterDriverFactory(siemensFactory, siemensValidator);

// 5️⃣ Driver Orchestrator
var orchestrator = new DriverOrchestrator(repository, driverManager);

// 6️⃣ Creamos un PLC
var plc = new PlcConnectionDefinition
{
    Id = Guid.NewGuid(),
    Name = "PLC Live",
    DriverType = "Siemens",
    IpAddress = "192.168.1.32",
    Rack = 0,
    Slot = 1,
    Tags = new List<PlcTagDefinition>
    {
        new() { Id = Guid.NewGuid(), Name = "Confirmar", Datatype = "Bool", Address = "DB1.DBX0.0" },
        new() { Id = Guid.NewGuid(), Name = "Finalizar", Datatype = "Bool", Address = "DB1.DBX0.1" },
        new() { Id = Guid.NewGuid(), Name = "Tanque 1", Datatype = "Real", Address = "DB1.DBD2" },
        new() { Id = Guid.NewGuid(), Name = "Presion", Datatype = "Real", Address = "DB1.DBD6" },
        new() { Id = Guid.NewGuid(), Name = "Presion 2", Datatype = "Real", Address = "DB3.DBD6" }
    }
};

// 7️⃣ Guardamos el PLC (esto arranca el ciclo de lectura)
await orchestrator.CreatePlcAsync(plc);

Console.WriteLine("✅ PLC created. Waiting for data...");

// 8️⃣ Damos unos segundos para que arranque el ciclo
await Task.Delay(5000);

// 9️⃣ Consultamos los valores
var diagnostics = driverManager.GetAllDrivers();
foreach (var diag in diagnostics)
{
    Console.WriteLine($"PLC: {diag.Name} | Connected: {diag.IsConnected}");
    foreach (var tag in diag.Tags)
    {
        Console.WriteLine($"   Tag: {tag.Name} = {tag.Value} (Quality: {tag.Quality})");
    }
}

Console.WriteLine("✅ Test finished.");