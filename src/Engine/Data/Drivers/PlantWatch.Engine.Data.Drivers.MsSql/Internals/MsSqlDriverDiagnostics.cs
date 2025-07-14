using Microsoft.Data.SqlClient;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Data.Drivers.MsSql.Internals;


public class MsSqlDriverDiagnostics : IDatabaseDriverDiagnostics
{
    public Guid Id { get; }
    public string Name { get; }
    public string DriverType => "SqlServer";
    public bool IsOnline { get; set; }
    public string? LastError { get; set; }


    public MsSqlDriverDiagnostics(Guid id, string name, bool isOnline, string? lastError = null)
    {
        Id = id;
        Name = name;
        IsOnline = isOnline;
        LastError = lastError;
    }
}
