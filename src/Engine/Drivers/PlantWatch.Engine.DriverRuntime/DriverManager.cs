using System;
using PlantWatch.Engine.Core.Factories;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.Core.Services.Drivers;
using PlantWatch.Engine.Core.Validators;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.DriverRuntime.Models;
using System.Text.Json;

namespace PlantWatch.DriverRuntime;

public interface IDriverManager
{
    Task ReloadDriversAsync();

    IEnumerable<IPLCService> GetAllDrivers();

    IPLCService GetDriver(Guid plcId);  // 🔥 Ya usando el ID único

    IEnumerable<IDriverDiagnostics> GetAllDiagnostics();

    IDriverDiagnostics GetDiagnostics(Guid plcId);

    Task<bool> WriteTagAsync(Guid plcId, Guid tagId, object value);

}
public class DriverManager : IDriverManager
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
        foreach (var driver in _plcDrivers)
            await driver.StopAsync();

        _plcDrivers.Clear();

        var configs = await _configRepository.LoadAllPlcConfigurationsAsync();

        foreach (var config in configs)
        {
            Console.WriteLine($"[DriverManager] Processing PLC configuration: {config.Name} ({config.Id})");
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
                await Task.Delay(1000);
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

    public IPLCService GetDriver(Guid plcId)
        => _plcDrivers.FirstOrDefault(d => d.Id == plcId);

    public IEnumerable<IDriverDiagnostics> GetAllDiagnostics()
        => _plcDrivers.OfType<IDriverDiagnostics>().ToList();

    public IDriverDiagnostics GetDiagnostics(Guid plcId)
    {
        var driver = _plcDrivers.FirstOrDefault(d => d.Id == plcId);
        return driver as IDriverDiagnostics;
    }

    public async Task<bool> WriteTagAsync(Guid plcId, Guid tagId, object value)
    {

        foreach (var driver in _plcDrivers)
        {
            Console.WriteLine($"[DriverManager] Checking driver {driver.Name} and Id: {driver.Id} for PLC {plcId}");
        }

        var plc = _plcDrivers.FirstOrDefault(p => p.Id == plcId);

        if (plc == null)
            throw new ArgumentException($"PLC not found: {plcId}");

        return await plc.WriteTagAsync(tagId, value);
    }
}