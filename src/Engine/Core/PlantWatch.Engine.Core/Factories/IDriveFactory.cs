using System;
using PlantWatch.Engine.Core.Models.Definitions;
using PlantWatch.Engine.Core.Services.Drivers;

namespace PlantWatch.Engine.Core.Factories;

public interface IDriverFactory
{
    string DriverType { get; }
    IPLCService CreateDriver(PlcConnectionDefinition config);
}