using System;
using System.Collections.Concurrent;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Data.Managers;



public class DatabaseDriverManager : IDatabaseDriverManager
{
    private readonly IDatabaseConfigurationRepository _configRepository;


    private readonly List<IDatabaseDriver> _drivers = new();

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

            _drivers.Add(driver);
        }
    }


    public IDatabaseDriver? GetDriver(Guid id)
    {
        return _drivers.FirstOrDefault(d => d.Id == id);
    }

    public bool ValidateCommand(Guid id, string command)
    {
        var driver = GetDriver(id);
        return driver?.ValidateCommand(command) ?? false;
    }

    public async Task<ExecutionResult> ExecuteAsync(Guid id, string command)
    {
        var driver = GetDriver(id);
        if (driver == null)
            return ExecutionResult.Fail($"Driver '{id}' not found.");

        return await driver.ExecuteAsync(command);
    }

    public IEnumerable<string> GetAvailableDrivers() => _drivers.ToList().Select(d => d.Name);

    public Dictionary<string, IDatabaseDriverDescriptor> GetDriverDescriptors()
    => _factories.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Value.GetDriverDescriptor(),
        StringComparer.OrdinalIgnoreCase);

    public IDatabaseDriverDiagnostics GetDiagnostics(Guid id)
    {
        var driver = _drivers.Where(x => x.Id == id).FirstOrDefault();
        return driver.GetDiagnostics() ?? null;
    }

    public IEnumerable<IDatabaseDriverDiagnostics> GetDiagnostics()
    {
        var driver = _drivers.ToList();
        return driver.Select(d => d.GetDiagnostics()).ToList();
    }

    public async Task<ExecutionResult> GetSchemaAsync(Guid id)
    {
        var driver = GetDriver(id);
        if (driver == null)
            return ExecutionResult.Fail($"Driver '{id}' not found.");

        return await driver.GetSchemaAsync();
    }

}