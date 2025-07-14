using System;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseDriver
{
    Guid Id { get; } // Unique identifier for the database driver
    string Name { get; }

    Task<bool> IsOnline();

    string GetDescriptor();

    bool ValidateCommand(string command);

    Task<ExecutionResult> ExecuteAsync(string command);
    IDatabaseDriverDiagnostics GetDiagnostics();
    Task<ExecutionResult> GetSchemaAsync();

}