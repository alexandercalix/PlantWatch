using System;
using System.Collections.Generic;
using PlantWatch.Engine.Drivers.Core.Descriptors;

namespace PlantWatch.Engine.Drivers.Protocols.Siemens.Descriptors;

public class SiemensPlcConfigurationDescriptor : IPlcConfigurationDescriptor
{
    public string DriverType => "Siemens";

    public IEnumerable<PlcParameterDescriptor> Parameters => new[]
    {
        new PlcParameterDescriptor { Name = "IpAddress", DisplayName = "IP Address", DataType = "string", IsRequired = true },
        new PlcParameterDescriptor { Name = "Rack", DisplayName = "Rack", DataType = "int", IsRequired = true },
        new PlcParameterDescriptor { Name = "Slot", DisplayName = "Slot", DataType = "int", IsRequired = true },
        new PlcParameterDescriptor { Name = "CpuType", DisplayName = "CPU Type", DataType = "enum", IsRequired = true, AllowedValues = new[] { "S71200", "S71500", "S7300", "S7400" } }
    };
}
