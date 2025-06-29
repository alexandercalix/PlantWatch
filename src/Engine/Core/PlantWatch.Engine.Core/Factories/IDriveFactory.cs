using System;
using PlantWatch.Core;
using PlantWatch.Core.Interfaces.Engine.Services;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.Core.Models.Definitions;
using PlantWatch.Engine.Core.Services.Drivers;
using PlantWatch.Engine.Drivers.Core.Descriptors;

namespace PlantWatch.Engine.Core.Factories;

public interface IDriverFactory
{
    string DriverType { get; }
    IDriverCapabilities GetCapabilities();
    IPlcConfigurationDescriptor GetPlcDescriptor();
    ITagValidationDescriptor GetTagDescriptor();

    IPLCService CreateDriver(PlcConnectionDefinition config);
}
