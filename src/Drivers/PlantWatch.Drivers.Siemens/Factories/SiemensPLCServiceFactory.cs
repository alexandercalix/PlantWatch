using System;
using System.Linq;
using PlantWatch.Core.Factories;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.Core.Services.Drivers;
using PlantWatch.Drivers.Siemens.Services;

namespace PlantWatch.Drivers.Siemens.Factories;

public class SiemensPLCServiceFactory : IDriverFactory
{
    public string DriverType => "Siemens";

    public IPLCService CreateDriver(PlcConnectionDefinition config)
    {
        Console.WriteLine($"[Factory] Creating SiemensPLCService for {config.Name} at {config.IpAddress}");

        var tags = config.Tags
            .Select(t => SiemensTagFactory.Create(
                t.Id,
                t.Name,
                t.Datatype,
                t.Address))
            .ToList();

        return new SiemensPLCService(
            id: config.Id,
            name: config.Name,
            ipAddress: config.IpAddress,
            rack: (short)config.Rack,
            slot: (short)config.Slot,
            tags: tags
        );
    }
}