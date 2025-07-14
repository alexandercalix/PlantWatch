using System;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface ICommandExecutor
{
    Task<ExecutionResult> ExecuteAsync(string command);
    Task<ExecutionResult> GetSchemaAsync();
}
