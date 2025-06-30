using System;
using Microsoft.Data.SqlClient;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Data.Drivers.MsSql.Internals;


public class MsSqlExecutor : ICommandExecutor
{
    private readonly string _connectionString;

    public MsSqlExecutor(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<ExecutionResult> ExecuteAsync(string command)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(command, conn);

            if (command.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                using var reader = await cmd.ExecuteReaderAsync();
                var result = new List<Dictionary<string, object>>();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    result.Add(row);
                }

                return ExecutionResult.Ok(result);
            }
            else
            {
                int affected = await cmd.ExecuteNonQueryAsync();
                return ExecutionResult.Ok(new { AffectedRows = affected });
            }
        }
        catch (Exception ex)
        {
            return ExecutionResult.Fail(ex.Message);
        }
    }
}