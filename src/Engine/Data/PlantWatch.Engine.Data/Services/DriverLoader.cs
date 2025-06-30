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
            if (db.DriverType.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            {
                if (db.Parameters.TryGetValue("ConnectionString", out var connStr))
                {
                    var validator = new MsSqlValidator();
                    var executor = new MsSqlExecutor(connStr);
                    var diagnostics = new MsSqlDiagnostics(connStr);

                    drivers.Add(new MsSqlDriver(db.Name, validator, executor, diagnostics));
                }
            }

            // 🚀 Aquí se añadirán más drivers internos (Influx, Mongo, etc.)
        }

        return drivers;
    }
}