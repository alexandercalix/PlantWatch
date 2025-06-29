using System;
using System.Collections.Generic;

namespace PlantWatch.Engine.Drivers.Core.Descriptors;

public interface IPlcConfigurationDescriptor
{
    string DriverType { get; }
    IEnumerable<PlcParameterDescriptor> Parameters { get; }
}
