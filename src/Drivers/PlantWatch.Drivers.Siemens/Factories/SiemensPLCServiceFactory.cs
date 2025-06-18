using System;
using System.Linq;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.Drivers.Siemens.Services;

namespace PlantWatch.Drivers.Siemens.Factories;

public static class SiemensPLCServiceFactory
{
    public static SiemensPLCService CreateFromConfig(PlcConnectionDefinition config)
    {
        Console.WriteLine($"[Factory] Creating SiemensPLCService for {config.Name} at {config.IpAddress}");

        var tags = config.Tags
            .Select(t => SiemensTagFactory.Create(t.Id, t.Name, t.Datatype, t.Address, ConvertToPlcType(t.Datatype, t.DefaultValue)))
            .ToList();

        return new SiemensPLCService(
            name: config.Name,
            ipAddress: config.IpAddress,
            rack: (short)config.Rack,
            slot: (short)config.Slot,
            tags: tags
        );
    }

    private static object ConvertToPlcType(string datatype, object value)
    {
        return datatype switch
        {
            "Bool" => Convert.ToBoolean(value),
            "Byte" => Convert.ToByte(value),
            "Word" => Convert.ToUInt16(value),
            "DWord" => Convert.ToUInt32(value),
            "Int" => Convert.ToInt16(value),
            "DInt" => Convert.ToInt32(value),
            "Real" => Convert.ToSingle(value),
            _ => throw new ArgumentException($"Unsupported datatype: {datatype}")
        };
    }
}