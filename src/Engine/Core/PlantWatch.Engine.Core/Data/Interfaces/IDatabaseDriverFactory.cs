using System;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseDriverFactory
{
    string DriverType { get; }
    bool CanHandle(string driverType);
    IDatabaseDriver Create(DatabaseConfig config);
    IDatabaseDriverDescriptor GetDriverDescriptor(); // <-- nuevo

}