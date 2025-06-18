using System;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Services.Drivers;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.DriverRuntime.Models;

namespace PlantWatch.DriverRuntime.Services;

public class DriverManager : IRuntimeDriverService
{
    private readonly IEnumerable<IDriverFactory> _factories;
    private readonly IConfigurationRepository _configRepository;

    private readonly List<IPLCService> _plcDrivers = new();

    public DriverManager(IEnumerable<IDriverFactory> factories, IConfigurationRepository configRepository)
    {
        _factories = factories;
        _configRepository = configRepository;
    }

    public async Task ReloadDriversAsync()
    {
        // Stop current drivers
        foreach (var driver in _plcDrivers)
        {
            await driver.StopAsync();
        }
        _plcDrivers.Clear();

        // Load config from DB
        var configs = await _configRepository.LoadAllPlcConfigurationsAsync();

        foreach (var config in configs)
        {
            var factory = _factories.FirstOrDefault(f => f.DriverType == config.DriverType);
            if (factory == null)
            {
                Console.WriteLine($"[DriverManager] No factory found for driver type: {config.DriverType}");
                continue;
            }

            var driver = factory.CreateDriver(config);
            _plcDrivers.Add(driver);
            await driver.StartAsync();
        }
    }

    public async Task<IEnumerable<string>> GetAvailablePlcsAsync()
    {
        return _plcDrivers.Select(d => d.Name);
    }

    public async Task<RuntimePlcSnapshot> GetPlcSnapshotAsync(string plcName)
    {
        var plc = _plcDrivers.FirstOrDefault(p => p.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
        if (plc == null) return null;

        return new RuntimePlcSnapshot
        {
            PlcName = plc.Name,
            IsConnected = plc.IsConnected,
            Tags = plc.Tags.Select(t => new RuntimeTagSnapshot
            {
                TagName = t.Name,
                Datatype = t.Datatype,
                Value = t.Value,
                Quality = t.Quality
            }).ToList()
        };
    }

    public async Task<RuntimeTagSnapshot> ReadTagAsync(string plcName, string tagName)
    {
        var plc = _plcDrivers.FirstOrDefault(p => p.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
        if (plc == null) return null;

        var tag = plc.Tags.FirstOrDefault(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (tag == null) return null;

        return new RuntimeTagSnapshot
        {
            TagName = tag.Name,
            Datatype = tag.Datatype,
            Value = tag.Value,
            Quality = tag.Quality
        };
    }

    public async Task<bool> WriteTagAsync(string plcName, string tagName, object value)
    {
        var plc = _plcDrivers.FirstOrDefault(p => p.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
        if (plc == null) return false;

        return await plc.WriteTagAsync(tagName, value);
    }

    public async Task<IEnumerable<RuntimeTagSnapshot>> GetAllTagsAsync()
    {
        var allTags = new List<RuntimeTagSnapshot>();

        foreach (var plc in _plcDrivers)
        {
            foreach (var tag in plc.Tags)
            {
                allTags.Add(new RuntimeTagSnapshot
                {
                    TagName = tag.Name,
                    Datatype = tag.Datatype,
                    Value = tag.Value,
                    Quality = tag.Quality
                });
            }
        }

        return allTags;
    }

}