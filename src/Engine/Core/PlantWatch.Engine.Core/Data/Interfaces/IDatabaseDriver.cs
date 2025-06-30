using System;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseDriver
{
    string Name { get; }

    bool IsOnline();

    string GetDescriptor();

    bool ValidateCommand(string command);

    Task<ExecutionResult> ExecuteAsync(string command);
}