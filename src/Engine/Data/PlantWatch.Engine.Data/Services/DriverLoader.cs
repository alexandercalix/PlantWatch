using System;
using System.Text.Json;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Drivers.MsSql;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Data.Services;

public static class DriverLoader
{
    public static List<IDatabaseDriver> LoadDriversFromJson(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Driver config file not found.", path);

        var json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new AppConfig();

        var drivers = new List<IDatabaseDriver>();

        foreach (var db in config.Databases)
        {
            if (_factoryManager.TryGetFactory(db.DriverType, out var factory))
            {
                try
                {
                    var driver = factory.Create(db);
                    drivers.Add(driver);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DataDriverLoader] Error creating driver '{db.Name}': {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"[DataDriverLoader] No factory for driver type: {db.DriverType}");
            }
        }

        return drivers;
    }
}