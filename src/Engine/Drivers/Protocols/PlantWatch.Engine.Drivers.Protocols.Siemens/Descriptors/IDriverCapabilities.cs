using System;
using System.Collections.Generic;
using PlantWatch.Engine.Core.Interfaces;

namespace PlantWatch.Engine.Drivers.Protocols.Siemens.Descriptors;



public class SiemensDriverCapabilities : IDriverCapabilities
{
    public string DriverType => "Siemens";

    public IEnumerable<string> SupportedDeviceModels => new[]
    {
        "S7-1200",
        "S7-1500",
        "S7-300",
        "S7-400",
        "S7-200 / LOGO (TSAP)"
    };

    public string Description => "Driver for Siemens S7 PLC family (using S7.Net protocol)";
}
