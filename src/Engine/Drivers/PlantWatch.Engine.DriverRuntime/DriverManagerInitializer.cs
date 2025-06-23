using System;
using Microsoft.Extensions.Hosting;

namespace PlantWatch.DriverRuntime;

public class DriverManagerInitializer : IHostedService
{
    private readonly IDriverManager _manager;

    public DriverManagerInitializer(IDriverManager manager)
    {
        _manager = manager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("👉 Starting DriverManager...");
        await _manager.ReloadDriversAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}