using PlantWatch.Core.Models.Definitions;
using PlantWatch.Core.Services.Drivers;

namespace PlantWatch.DriverRuntime.Interfaces;

public interface IDriverFactory
{
    string DriverType { get; } // Por ejemplo: "Siemens", "Modbus", "AllenBradley"
    IPLCService CreateDriver(PlcConnectionDefinition config);
}