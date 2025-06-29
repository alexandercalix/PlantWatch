using System;
using System.Collections.Generic;

namespace PlantWatch.Engine.Core.Interfaces;

public interface IDriverCapabilities
{
    string DriverType { get; }
    IEnumerable<string> SupportedDeviceModels { get; }
    string Description { get; }
}
