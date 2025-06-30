using Microsoft.Data.SqlClient;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Data.Drivers.MsSql.Internals;


public class MsSqlDiagnostics : IDatabaseDiagnostics
{
    private readonly string _connectionString;

    public MsSqlDiagnostics(string connectionString)
    {
        _connectionString = connectionString;
    }

    public bool IsOnline()
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
}