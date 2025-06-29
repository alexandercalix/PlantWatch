using System;
using System.Linq;
using PlantWatch.Core;
using PlantWatch.Core.Interfaces.Engine.Services;
using PlantWatch.Engine.Core.Factories;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.Core.Models.Definitions;
using PlantWatch.Engine.Drivers.Core.Descriptors;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Descriptors;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Services;

namespace PlantWatch.Engine.Drivers.Protocols.Siemens.Factories;

public class SiemensPLCServiceFactory : IDriverFactory
{
    public string DriverType => "Siemens";

    public IDriverCapabilities GetCapabilities() => new SiemensDriverCapabilities();
    public IPlcConfigurationDescriptor GetPlcDescriptor() => new SiemensPlcConfigurationDescriptor();
    public ITagValidationDescriptor GetTagDescriptor() => new SiemensTagValidationDescriptor();

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
