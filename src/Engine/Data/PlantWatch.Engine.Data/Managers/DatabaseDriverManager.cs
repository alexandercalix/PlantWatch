using System;
using System.Collections.Concurrent;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Data.Managers;

public class DatabaseDriverManager
{
    private readonly ConcurrentDictionary<string, IDatabaseDriver> _drivers = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterDriver(IDatabaseDriver driver)
    {
        _drivers[driver.Name] = driver;
    }

    public IDatabaseDriver? GetDriver(string name)
    {
        _drivers.TryGetValue(name, out var driver);
        return driver;
    }

    public bool ValidateCommand(string driverName, string command)
    {
        var driver = GetDriver(driverName);
        return driver?.ValidateCommand(command) ?? false;
    }

    public async Task<ExecutionResult> ExecuteAsync(string driverName, string command)
    {
        var driver = GetDriver(driverName);
        if (driver == null)
            return ExecutionResult.Fail($"Driver '{driverName}' not found.");

        if (!driver.IsOnline())
            return ExecutionResult.Fail($"Driver '{driverName}' is offline.");

        if (!driver.ValidateCommand(command))
            return ExecutionResult.Fail($"Invalid command for '{driverName}'.");

        return await driver.ExecuteAsync(command);
    }

    public IEnumerable<string> GetAvailableDrivers() => _drivers.Keys;

    public Dictionary<string, string> GetDriverDescriptors()
        => _drivers.ToDictionary(d => d.Key, d => d.Value.GetDescriptor());
}