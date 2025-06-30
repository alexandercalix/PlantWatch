using System;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseDriverFactory
{
    bool CanHandle(string driverType);
    IDatabaseDriver Create(DatabaseConfig config);
}