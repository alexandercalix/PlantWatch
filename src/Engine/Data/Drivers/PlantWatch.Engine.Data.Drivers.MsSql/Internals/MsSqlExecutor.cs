using System;
using Microsoft.Data.SqlClient;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Core.Data.Models.Schema;

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

    public async Task<ExecutionResult> GetSchemaAsync()
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var command = @"
        SELECT 
            c.TABLE_NAME,
            c.COLUMN_NAME,
            c.DATA_TYPE,
            c.IS_NULLABLE,
            c.CHARACTER_MAXIMUM_LENGTH,
            c.NUMERIC_PRECISION,
            c.NUMERIC_SCALE,
            pk.COLUMN_NAME AS PRIMARY_KEY
        FROM INFORMATION_SCHEMA.COLUMNS c
        LEFT JOIN (
            SELECT ku.TABLE_NAME, ku.COLUMN_NAME
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
            WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
        ) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME
        ORDER BY c.TABLE_NAME, c.ORDINAL_POSITION;
        ";

            using var cmd = new SqlCommand(command, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var schema = new DatabaseSchema();
            string currentTable = null;
            DatabaseTable table = null;

            while (await reader.ReadAsync())
            {
                var tableName = reader["TABLE_NAME"].ToString();
                if (currentTable != tableName)
                {
                    currentTable = tableName;
                    table = new DatabaseTable { Name = tableName, Type = "table" };
                    schema.Tables.Add(table);
                }

                table.Fields.Add(new DatabaseField
                {
                    Name = reader["COLUMN_NAME"].ToString(),
                    Type = reader["DATA_TYPE"].ToString(),
                    IsNullable = reader["IS_NULLABLE"].ToString().Equals("YES", StringComparison.OrdinalIgnoreCase),
                    MaxLength = reader["CHARACTER_MAXIMUM_LENGTH"] is DBNull ? null : (int?)Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]),
                    Precision = reader["NUMERIC_PRECISION"] is DBNull ? null : (int?)Convert.ToInt32(reader["NUMERIC_PRECISION"]),
                    Scale = reader["NUMERIC_SCALE"] is DBNull ? null : (int?)Convert.ToInt32(reader["NUMERIC_SCALE"]),
                    IsPrimaryKey = reader["PRIMARY_KEY"] != DBNull.Value
                });
            }

            return ExecutionResult.Ok(schema);
        }
        catch (Exception ex)
        {
            return ExecutionResult.Fail(ex.Message);
        }
    }



}