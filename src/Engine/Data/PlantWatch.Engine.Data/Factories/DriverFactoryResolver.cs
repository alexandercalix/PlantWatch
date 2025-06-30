using System;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Data.Factories;

public class DriverFactoryResolver
{
    private readonly List<IDatabaseDriverFactory> _factories = new();

    public void RegisterFactory(IDatabaseDriverFactory factory)
    {
        _factories.Add(factory);
    }

    public IDatabaseDriver? CreateDriver(DatabaseConfig config)
    {
        var factory = _factories.FirstOrDefault(f => f.CanHandle(config.DriverType));
        return factory?.Create(config);
    }
}