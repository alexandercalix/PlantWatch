using System;
using PlantWatch.Engine.Data.Drivers.MsSql;
using PlantWatch.Engine.Data.Factories;

namespace PlantWatch.Engine.Data.Services;

public static class EngineDataBootstrapper
{
    public static DriverFactoryResolver CreateDefaultFactoryResolver()
    {
        var resolver = new DriverFactoryResolver();
        resolver.RegisterFactory(new MsSqlDriverFactory());
        // Future: resolver.RegisterFactory(new InfluxDriverFactory());
        return resolver;
    }
}
