using System;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.Core.Services.Drivers;

namespace PlantWatch.Core.Factories;

public interface IDriverFactory
{
    string DriverType { get; }
    IPLCService CreateDriver(PlcConnectionDefinition config);
}