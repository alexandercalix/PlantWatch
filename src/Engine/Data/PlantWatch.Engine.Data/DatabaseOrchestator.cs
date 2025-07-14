using System;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Managers;

namespace PlantWatch.Engine.Data;

public interface IDatabaseDriverOrchestrator
{
    Task CreateDatabaseAsync(DatabaseConfig config);
    Task UpdateDatabaseAsync(DatabaseConfig config);
    Task DeleteDatabaseAsync(Guid id);
    Task<IEnumerable<IDatabaseDriverDiagnostics>> GetAllDatabaseDiagnosticsAsync();
    Task<IDatabaseDriverDiagnostics?> GetDatabaseDiagnosticsAsync(Guid id);

}

public class DatabaseDriverOrchestrator : IDatabaseDriverOrchestrator
{
    private readonly IDatabaseConfigurationRepository _repository;
    private readonly IDatabaseDriverManager _driverManager;

    public DatabaseDriverOrchestrator(
        IDatabaseConfigurationRepository repository,
        IDatabaseDriverManager driverManager)
    {
        _repository = repository;
        _driverManager = driverManager;
    }

    public async Task CreateDatabaseAsync(DatabaseConfig config)
    {
        if (config.Id == Guid.Empty)
            config.Id = Guid.NewGuid();

        // TODO: Optionally validate config parameters here in the future

        await _repository.SaveDatabaseConfigurationAsync(config);
        await _driverManager.ReloadDriversAsync();
    }

    public async Task UpdateDatabaseAsync(DatabaseConfig config)
    {
        if (config.Id == Guid.Empty)
            throw new InvalidOperationException("Database ID cannot be empty for update.");

        await _repository.SaveDatabaseConfigurationAsync(config);
        await _driverManager.ReloadDriversAsync();
    }

    public async Task DeleteDatabaseAsync(Guid id)
    {
        var configs = await _repository.LoadAllDatabaseConfigurationsAsync();
        var db = configs.FirstOrDefault(c => c.Id == id);
        if (db != null)
        {
            await _repository.DeleteDatabaseConfigurationAsync(id);
            await _driverManager.ReloadDriversAsync();
        }
    }

    public async Task<IEnumerable<IDatabaseDriverDiagnostics>> GetAllDatabaseDiagnosticsAsync()
    {
        // Simulating async for future flexibility
        return await Task.FromResult(_driverManager.GetDiagnostics());
    }

    public async Task<IDatabaseDriverDiagnostics?> GetDatabaseDiagnosticsAsync(Guid id)
    {
        return await Task.FromResult(_driverManager.GetDiagnostics(id));
    }

}

