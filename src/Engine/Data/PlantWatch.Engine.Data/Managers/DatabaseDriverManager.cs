using System;
using System.Collections.Concurrent;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Data.Managers;

public class DatabaseDriverManager
{
    private readonly IDatabaseConfigurationRepository _configRepository;


    private readonly ConcurrentDictionary<string, IDatabaseDriver> _drivers = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, IDatabaseDriverFactory> _factories = new(StringComparer.OrdinalIgnoreCase);

    public DatabaseDriverManager(IDatabaseConfigurationRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public void RegisterDriverFactory(IDatabaseDriverFactory factory)
    {

        _factories[factory.DriverType] = factory;
        Console.WriteLine($"[DriverManager] Registered driver factory for {factory.DriverType}");
    }

    public async Task ReloadDriversAsync()
    {
        _drivers.Clear();

        var configs = await _configRepository.LoadAllDatabaseConfigurationsAsync();

        foreach (var config in configs)
        {
            if (!_factories.TryGetValue(config.DriverType, out var factory))
                throw new InvalidOperationException($"No factory registered for driver type '{config.DriverType}'");

            var driver = factory.Create(config);
            if (driver == null)
                throw new InvalidOperationException($"Failed to create driver for '{config.Name}'");

            _drivers[config.Name] = driver;
        }
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

        return await driver.ExecuteAsync(command);
    }

    public IEnumerable<string> GetAvailableDrivers() => _drivers.Keys;

    public Dictionary<string, string> GetDriverDescriptors()
        => _drivers.ToDictionary(d => d.Key, d => d.Value.GetDescriptor());
}