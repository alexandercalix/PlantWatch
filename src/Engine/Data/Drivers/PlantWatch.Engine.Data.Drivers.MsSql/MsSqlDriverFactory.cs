using System;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Data.Drivers.MsSql;

public class MsSqlDriverFactory : IDatabaseDriverFactory
{
    public bool CanHandle(string driverType) =>
        driverType.Equals("SqlServer", StringComparison.OrdinalIgnoreCase);

    public IDatabaseDriver Create(DatabaseConfig config)
    {
        if (!config.Parameters.TryGetValue("ConnectionString", out var connStr))
            throw new ArgumentException("Missing 'ConnectionString' in config.");

        var validator = new MsSqlValidator();
        var executor = new MsSqlExecutor(connStr);
        var diagnostics = new MsSqlDiagnostics(connStr);

        return new MsSqlDriver(
     config.Name,
     new MsSqlValidator(),
     new MsSqlExecutor(connStr),
     new MsSqlDiagnostics(connStr)
 );
    }
}
