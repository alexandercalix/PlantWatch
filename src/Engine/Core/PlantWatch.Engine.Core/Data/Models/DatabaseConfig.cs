using System;
using System.Collections.Generic;

namespace PlantWatch.Engine.Core.Data.Models;

public class DatabaseConfig
{
    public string Name { get; set; } = string.Empty;
    public string DriverType { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new();
}

public class AppConfig
{
    public List<DatabaseConfig> Databases { get; set; } = new();
}