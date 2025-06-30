using System;

namespace PlantWatch.Engine.Data.Services;

public static class ConfigurationPaths
{
    public static string DriversJsonPath => Path.Combine(AppContext.BaseDirectory, "drivers.json");
}