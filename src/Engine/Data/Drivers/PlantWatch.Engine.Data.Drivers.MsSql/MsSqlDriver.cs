using System;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Data.Drivers.MsSql;

public class MsSqlDriver : IDatabaseDriver
{
    public string Name { get; }

    private readonly ICommandValidator _validator;
    private readonly ICommandExecutor _executor;
    private readonly IDatabaseDiagnostics _diagnostics;

    public MsSqlDriver(string name, ICommandValidator validator, ICommandExecutor executor, IDatabaseDiagnostics diagnostics)
    {
        Name = name;
        _validator = validator;
        _executor = executor;
        _diagnostics = diagnostics;
    }

    public bool IsOnline() => _diagnostics.IsOnline();

    public string GetDescriptor() => "Microsoft SQL Server Driver (Split Core)";

    public bool ValidateCommand(string command) => _validator.Validate(command);

    public Task<ExecutionResult> ExecuteAsync(string command) => _executor.ExecuteAsync(command);
}