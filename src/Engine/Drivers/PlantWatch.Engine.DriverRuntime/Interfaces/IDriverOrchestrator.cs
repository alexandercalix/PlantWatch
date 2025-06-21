using System;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.Core.Models.Definitions;

namespace PlantWatch.DriverRuntime.Interfaces;

public interface IDriverOrchestrator
{
    // PLC CRUD
    Task CreatePlcAsync(PlcConnectionDefinition plc);
    Task UpdatePlcAsync(PlcConnectionDefinition plc);
    Task DeletePlcAsync(Guid plcId);

    // Tag CRUD
    Task AddOrUpdateTagAsync(Guid plcId, PlcTagDefinition tag);
    Task DeleteTagAsync(Guid plcId, Guid tagId);

    // Monitoreo (acceso a runtime)
    Task<IEnumerable<IDriverDiagnostics>> GetDiagnosticsAsync();
    Task<IDriverDiagnostics> GetDiagnosticsAsync(Guid plcId);
}
