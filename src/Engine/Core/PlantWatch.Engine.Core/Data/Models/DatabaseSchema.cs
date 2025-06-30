using System;
using System.Collections.Generic;

namespace PlantWatch.Engine.Core.Data.Models;

public class DatabaseSchema
{
    public string DatabaseName { get; set; } = string.Empty;
    public List<DbTable> Tables { get; set; } = new();
    public List<DbRelationship> Relationships { get; set; } = new();
}

public class DbTable
{
    public string Name { get; set; } = string.Empty;
    public List<DbColumn> Columns { get; set; } = new();
}

public class DbColumn
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
}

public class DbRelationship
{
    public string FromTable { get; set; } = string.Empty;
    public string FromColumn { get; set; } = string.Empty;
    public string ToTable { get; set; } = string.Empty;
    public string ToColumn { get; set; } = string.Empty;
}
