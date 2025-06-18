using System;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Services.Drivers;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.DriverRuntime.Models;

namespace PlantWatch.DriverRuntime;

public class DriverManager
{
    private readonly IConfigurationRepository _configRepository;
    private readonly Dictionary<string, (IDriverFactory Factory, IConfigurationValidator Validator)> _driverHandlers;
    private readonly List<IPLCService> _plcDrivers = new();

    public DriverManager(IConfigurationRepository configRepository)
    {
        _configRepository = configRepository;
        _driverHandlers = new Dictionary<string, (IDriverFactory, IConfigurationValidator)>(StringComparer.OrdinalIgnoreCase);
    }

    public void RegisterDriverFactory(IDriverFactory factory, IConfigurationValidator validator)
    {
        _driverHandlers[factory.DriverType] = (factory, validator);
        Console.WriteLine($"[DriverManager] Registered driver factory for {factory.DriverType}");
    }

    public async Task ReloadDriversAsync()
    {
        // Stop current drivers
        foreach (var driver in _plcDrivers)
        {
            await driver.StopAsync();
        }
        _plcDrivers.Clear();

        // Load config from repository
        var configs = await _configRepository.LoadAllPlcConfigurationsAsync();

        foreach (var config in configs)
        {
            if (!_driverHandlers.TryGetValue(config.DriverType, out var handler))
            {
                Console.WriteLine($"[DriverManager] No factory registered for driver type: {config.DriverType}");
                continue;
            }

            try
            {
                await handler.Validator.ValidatePlcDefinitionAsync(config);
                var driver = handler.Factory.CreateDriver(config);
                _plcDrivers.Add(driver);
                await driver.StartAsync();
                Console.WriteLine($"[DriverManager] Started driver for PLC '{config.Name}'");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DriverManager] Failed to start PLC '{config.Name}': {ex.Message}");
            }
        }
    }

    public IEnumerable<IPLCService> GetAllDrivers() => _plcDrivers;

    public IPLCService GetDriver(string plcName)
        => _plcDrivers.FirstOrDefault(d => d.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
}