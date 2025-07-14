using System;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseDriverDiagnostics
{
    Guid Id { get; }                // Unique Database Config ID
    string Name { get; }            // Friendly name of the database connection
    string DriverType { get; }      // SQL Server, InfluxDB, Redis, etc.
    bool IsOnline { get; set; }          // Whether it's currently online/available
    string? LastError { get; set; }      // Optional last error message (nullable)
}
