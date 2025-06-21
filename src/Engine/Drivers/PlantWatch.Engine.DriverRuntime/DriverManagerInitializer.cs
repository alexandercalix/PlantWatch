using System;
using Microsoft.Extensions.Hosting;

namespace PlantWatch.DriverRuntime;

public class DriverManagerInitializer : IHostedService
{
    private readonly DriverManager _driverManager;

    public DriverManagerInitializer(DriverManager driverManager)
    {
        _driverManager = driverManager;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[DriverRuntime] Initializing drivers...");
        await _driverManager.ReloadDriversAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Aquí podrías hacer un shutdown más elegante si lo deseas
        return Task.CompletedTask;
    }
}
