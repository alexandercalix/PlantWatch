using PlantWatch.Engine.Core.Models.Definitions;
using PlantWatch.Engine.Core.Models.Drivers;
using PlantWatch.Engine.Drivers.Siemens.Factories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

var config = new PlcConnectionDefinition
{
    Name = "PLC Live",
    IpAddress = "192.168.1.32",
    Rack = 0,
    Slot = 1,
    Tags = new List<PlcTagDefinition>
    {
        new() { Id = Guid.NewGuid(), Name = "Confirmar", Datatype = "Bool", Address = "DB1.DBX0.0", DefaultValue = false },
        new() { Id = Guid.NewGuid(), Name = "Finalizar", Datatype = "Bool", Address = "DB1.DBX0.1", DefaultValue = false },
        new() { Id = Guid.NewGuid(), Name = "Tanque 1", Datatype = "Real", Address = "DB1.DBD2", DefaultValue = 0f },
        new() { Id = Guid.NewGuid(), Name = "Presion", Datatype = "Real", Address = "DB1.DBD6", DefaultValue = 0f },
        new() { Id = Guid.NewGuid(), Name = "Presion 2", Datatype = "Real", Address = "DB3.DBD6", DefaultValue = 0f }
    }
};

// // Crear el driver usando el factory ya adaptado a la nueva estructura
// var plc = SiemensPLCServiceFactory.

// Console.WriteLine("🚀 Starting PLC cycle...");
// await plc.StartAsync();

// Console.WriteLine("⌛ Reading values for 3 seconds...");
// await Task.Delay(3000);

// // Durante el ciclo, consultamos los tags activos
// while (plc.IsRunning)
// {
//     if (plc.IsConnected)
//     {
//         foreach (var tag in plc.Tags)
//         {
//             Console.WriteLine($"Tag ID: {tag.Id}, Name: {tag.Name}, Value: {tag.Value}, Quality: {tag.Quality}");
//         }
//         Console.WriteLine("===========================");
//         await Task.Delay(1000);
//     }
//     else
//     {
//         Console.WriteLine("PLC is NOT connected.");
//     }
// }

// foreach (var tag in plc.Tags)
// {
//     Console.WriteLine($"🔹 {tag.Name} = {tag.Value} (Quality: {tag.Quality})");
// }

// await plc.StopAsync();
// Console.WriteLine("✅ Stopped.");
