using System;
using PlantWatch.DriverRuntime.Models;

namespace PlantWatch.DriverRuntime.Interfaces;

public interface IRuntimeDriverService
{
    Task<IEnumerable<string>> GetAvailablePlcsAsync();
    Task<RuntimePlcSnapshot> GetPlcSnapshotAsync(string plcName);
    Task<RuntimeTagSnapshot> ReadTagAsync(string plcName, string tagName);
    Task<bool> WriteTagAsync(string plcName, string tagName, object value);
    Task ReloadDriversAsync(); // Permite reiniciar todo el runtime en caliente
    Task<IEnumerable<RuntimeTagSnapshot>> GetAllTagsAsync();

}
