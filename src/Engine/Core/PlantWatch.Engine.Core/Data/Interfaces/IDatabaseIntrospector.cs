using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseIntrospector
{
    Task<DatabaseSchema> GetFullSchemaAsync();
}

public interface IScriptAnalyzer
{
    Task<List<SqlColumnInfo>> AnalyzeSelectQueryAsync(string sql);
}