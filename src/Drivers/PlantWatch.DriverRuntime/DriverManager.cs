using System;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Services.Drivers;
using PlantWatch.Core.Validators;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.DriverRuntime.Models;

namespace PlantWatch.DriverRuntime;

public interface IDriverManager
{
    Task ReloadDriversAsync();

    IEnumerable<IPLCService> GetAllDrivers();

    IPLCService GetDriver(Guid plcId);  // 🔥 Ya usando el ID único

    IEnumerable<IDriverDiagnostics> GetAllDiagnostics();

    IDriverDiagnostics GetDiagnostics(Guid plcId);
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

    public IPLCService GetDriver(Guid plcId)
        => _plcDrivers.FirstOrDefault(d => d.Id == plcId);

    public IEnumerable<IDriverDiagnostics> GetAllDiagnostics()
        => _plcDrivers.OfType<IDriverDiagnostics>().ToList();

    public IDriverDiagnostics GetDiagnostics(Guid plcId)
    {
        var driver = _plcDrivers.FirstOrDefault(d => d.Id == plcId);
        return driver as IDriverDiagnostics;
    }
}