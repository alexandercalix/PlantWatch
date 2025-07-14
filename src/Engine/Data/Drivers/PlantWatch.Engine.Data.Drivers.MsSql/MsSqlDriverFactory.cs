using System;
using PlantWatch.Engine.Core.Data;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Data.Drivers.MsSql;

public class MsSqlDriverFactory : IDatabaseDriverFactory
{
    public string DriverType => "SqlServer";

    public bool CanHandle(string driverType) =>
        driverType.Equals("SqlServer", StringComparison.OrdinalIgnoreCase);

    public IDatabaseDriver Create(DatabaseConfig config)
    {
        if (!config.Parameters.TryGetValue("ConnectionString", out var connStr))
            throw new ArgumentException("Missing 'ConnectionString' in config.");

        var validator = new MsSqlValidator();
        var executor = new MsSqlExecutor(connStr);
        var diagnostics = new MsSqlDriverDiagnostics(id: config.Id,
            name: config.Name,
            isOnline: false, // Initial state, will be updated by diagnostics
            lastError: null); // No errors initially

        return new MsSqlDriver(
            id: config.Id,
            name: config.Name,
            validator: validator,
            executor: executor,
            diagnostics: diagnostics
        );
    }

    public IDatabaseDriverDescriptor GetDriverDescriptor()
    {
        return new DriverDescriptor
        {
            DriverType = "SqlServer",
            FriendlyName = "Microsoft SQL Server",
            Parameters = new List<DriverParameterDefinition>
            {
                new()
                {
                    Key = "ConnectionString",
                    DisplayName = "Connection String",
                    Type = "string",
                    IsRequired = true,
                    Placeholder = "Server=localhost;Database=Test;User Id=sa;Password=pass;"
                }
            }
        };
    }
}

