using PlantWatch.Core.Models.Definitions;
using PlantWatch.Core.Models.Drivers;
using PlantWatch.Drivers.Siemens.Factories;

var config = new PlcConnectionDefinition
{
    Name = "PLC Live",
    IpAddress = "192.168.1.32",
    Rack = 0,
    Slot = 1,
    Tags = new List<PlcTagDefinition>
    {
        new() { Name = "Confirmar", Datatype = "Bool", Address = "DB1.DBX0.0", DefaultValue = false },
        new() { Name = "Finalizar", Datatype = "Bool", Address = "DB1.DBX0.1", DefaultValue = false },
        new() { Name = "Tanque 1", Datatype = "Real", Address = "DB1.DBD2", DefaultValue = 0f },
        new() { Name = "Presion", Datatype = "Real", Address = "DB1.DBD6", DefaultValue = 0f },
        new() { Name = "Presion 2", Datatype = "Real", Address = "DB3.DBD6", DefaultValue = 0f }
    }
};

var plc = SiemensPLCServiceFactory.CreateFromConfig(config);

Console.WriteLine("🚀 Starting PLC cycle...");
await plc.StartAsync();

Console.WriteLine("⌛ Reading values for 3 seconds...");
await Task.Delay(3000);

foreach (var tag in plc.Tags)
{
    Console.WriteLine($"🔹 {tag.Name} = {tag.Value} (Quality: {tag.Quality})");
}

await plc.StopAsync();
Console.WriteLine("✅ Stopped.");
