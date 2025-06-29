using System;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Interfaces.Engine.Models;

namespace PlantWatch.Core.Interfaces.Engine.Services;


public interface IPLCService
{
    Guid Id { get; }
    string Name { get; }
    bool IsConnected { get; }
    bool IsRunning { get; }

    IEnumerable<ITag> Tags { get; }

    Task StartAsync();
    Task StopAsync();
    Task<bool> WriteTagAsync(Guid Id, object value);

    void ForceRemap();
}

