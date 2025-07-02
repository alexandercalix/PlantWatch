using System;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Core.Factories;

namespace PlantWatch.Engine.Data.Factories;

public class DriverFactoryResolver
{
    private readonly Dictionary<string, IDatabaseDriverFactory> _factories = new(StringComparer.OrdinalIgnoreCase);

    public void RegisterFactory(IDatabaseDriverFactory factory)
    {
        if (string.IsNullOrWhiteSpace(factory.DriverType))
            throw new ArgumentException("Factory must specify a DriverType.");

        _factories[factory.DriverType] = factory;
    }

    public bool TryCreateDriver(DatabaseConfig config, out IDatabaseDriver? driver)
    {
        if (_factories.TryGetValue(config.DriverType, out var factory))
        {
            driver = factory.Create(config);
            return true;
        }

        driver = null;
        return false;
    }

    public IDatabaseDriver? CreateDriver(DatabaseConfig config)
    {
        return TryCreateDriver(config, out var driver) ? driver : null;
    }

    public IReadOnlyCollection<string> GetRegisteredDriverTypes()
    {
        return _factories.Keys;
    }
}