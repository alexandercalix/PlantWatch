using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseDriverManager
{
    Task<ExecutionResult> ExecuteAsync(Guid id, string command);
    IEnumerable<string> GetAvailableDrivers();
    IDatabaseDriver? GetDriver(Guid id);
    Dictionary<string, IDatabaseDriverDescriptor> GetDriverDescriptors();
    void RegisterDriverFactory(IDatabaseDriverFactory factory);
    Task ReloadDriversAsync();
    bool ValidateCommand(Guid id, string command);
    IEnumerable<IDatabaseDriverDiagnostics> GetDiagnostics();

    IDatabaseDriverDiagnostics GetDiagnostics(Guid id);

    Task<ExecutionResult> GetSchemaAsync(Guid id);
}