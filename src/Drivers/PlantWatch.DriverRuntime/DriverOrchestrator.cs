using System;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.DriverRuntime.Interfaces;

namespace PlantWatch.DriverRuntime;

public class DriverOrchestrator : IDriverOrchestrator
{
    private readonly IConfigurationRepository _repository;
    private readonly IDriverManager _driverManager;

    public DriverOrchestrator(IConfigurationRepository repository, IDriverManager driverManager)
    {
        _repository = repository;
        _driverManager = driverManager;
    }

    // --------------- PLC CRUD -----------------

    public async Task CreatePlcAsync(PlcConnectionDefinition plc)
    {
        if (plc.Id == Guid.Empty)
            plc.Id = Guid.NewGuid();

        await _repository.SavePlcConfigurationAsync(plc);
        await _driverManager.ReloadDriversAsync();
    }

    public async Task UpdatePlcAsync(PlcConnectionDefinition plc)
    {
        if (plc.Id == Guid.Empty)
            throw new InvalidOperationException("PLC ID cannot be empty for update.");

        await _repository.SavePlcConfigurationAsync(plc);
        await _driverManager.ReloadDriversAsync();
    }

    public async Task DeletePlcAsync(Guid plcId)
    {
        var existingPlc = await _repository.LoadAllPlcConfigurationsAsync();
        var plc = existingPlc.FirstOrDefault(p => p.Id == plcId);
        if (plc != null)
        {
            await _repository.DeletePlcConfigurationAsync(plc.Id);
            await _driverManager.ReloadDriversAsync();
        }
    }

    // --------------- TAG CRUD -----------------

    public async Task AddOrUpdateTagAsync(Guid plcId, PlcTagDefinition tag)
    {
        if (tag.Id == Guid.Empty)
            tag.Id = Guid.NewGuid();

        var existingPlc = await _repository.LoadAllPlcConfigurationsAsync();
        var plc = existingPlc.FirstOrDefault(p => p.Id == plcId);
        if (plc == null)
            throw new InvalidOperationException("PLC not found");

        await _repository.AddOrUpdateTagAsync(plc.Id, tag);
        await _driverManager.ReloadDriversAsync();
    }

    public async Task DeleteTagAsync(Guid plcId, Guid tagId)
    {
        var existingPlc = await _repository.LoadAllPlcConfigurationsAsync();
        var plc = existingPlc.FirstOrDefault(p => p.Id == plcId);
        if (plc == null)
            throw new InvalidOperationException("PLC not found");

        await _repository.DeleteTagAsync(plc.Id, tagId);
        await _driverManager.ReloadDriversAsync();
    }

    // --------------- Diagnóstico -----------------

    public async Task<IEnumerable<IDriverDiagnostics>> GetDiagnosticsAsync()
    {
        return _driverManager.GetAllDiagnostics();
    }

    public async Task<IDriverDiagnostics> GetDiagnosticsAsync(Guid plcId)
    {
        return _driverManager.GetDiagnostics(plcId);
    }
}