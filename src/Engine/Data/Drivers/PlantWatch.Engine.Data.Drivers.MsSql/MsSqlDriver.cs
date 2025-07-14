using System;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Data.Drivers.MsSql;

public class MsSqlDriver : IDatabaseDriver
{
    public Guid Id { get; }
    public string Name { get; }

    private readonly ICommandValidator _validator;
    private readonly ICommandExecutor _executor;
    private readonly IDatabaseDriverDiagnostics _diagnostics;

    public MsSqlDriver(Guid id, string name, ICommandValidator validator, ICommandExecutor executor, IDatabaseDriverDiagnostics diagnostics)
    {
        Id = id;
        Name = name;
        _validator = validator;
        _executor = executor;
        _diagnostics = diagnostics;
    }

    public async Task<bool> IsOnline()
    {
        try
        {
            await ExecuteAsync("SELECT 1");
            return true;
        }
        catch (Exception ex)
        {
            _diagnostics.LastError = ex.Message;
            return false;
        }
    }

    public string GetDescriptor() => "Microsoft SQL Server Driver (Split Core)";

    public bool ValidateCommand(string command) => _validator.Validate(command);

    public Task<ExecutionResult> ExecuteAsync(string command)
    {
        var result = _executor.ExecuteAsync(command);
        if (result.IsFaulted)
        {
            _diagnostics.LastError = result.Exception?.Message;
        }
        else
        {
            _diagnostics.IsOnline = true; // Update diagnostics if command executed successfully
        }
        return result;
    }
    public Task<ExecutionResult> GetSchemaAsync()
    {
        if (_executor is MsSqlExecutor mssql)
        {
            return mssql.GetSchemaAsync();
        }

        return Task.FromResult(ExecutionResult.Fail("Schema inspection not supported by this driver."));
    }
    public IDatabaseDriverDiagnostics GetDiagnostics()
    {
        return new MsSqlDriverDiagnostics(
            id: this.Id,
            name: this.Name,
            isOnline: _diagnostics.IsOnline,
            lastError: null // You can capture and store actual errors later if needed
        );
    }

}