using System;
using System.Data;
using Microsoft.Data.SqlClient;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Data.Drivers.MsSql.Internals;




public class MsSqlScriptAnalyzer : IScriptAnalyzer
{
    private readonly string _connectionString;

    public MsSqlScriptAnalyzer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<SqlColumnInfo>> AnalyzeSelectQueryAsync(string sql)
    {
        var columns = new List<SqlColumnInfo>();

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        // Force a limited run to extract schema without reading actual data
        using var cmd = new SqlCommand(sql + " OPTION (FAST 1)", conn);
        using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.SchemaOnly);

        var schema = reader.GetSchemaTable();

        if (schema == null)
            return columns;

        foreach (DataRow row in schema.Rows)
        {
            columns.Add(new SqlColumnInfo
            {
                Column = row["ColumnName"]?.ToString() ?? "",
                DataType = row["DataTypeName"]?.ToString() ?? row["DataType"]?.ToString() ?? "unknown",
                IsNullable = row["AllowDBNull"] is bool b && b,
                Table = row["BaseTableName"]?.ToString() ?? "unknown"
            });
        }

        return columns;
    }
}

public interface IScriptAnalyzer
{
}