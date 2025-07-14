using System;
using System.Collections.Generic;

namespace PlantWatch.Engine.Core.Data.Models.Schema;

public class DatabaseSchema
{
    public List<DatabaseTable> Tables { get; set; } = new();
}

public class DatabaseTable
{
    public string Name { get; set; }
    public string Type { get; set; }
    public List<DatabaseField> Fields { get; set; } = new();
    public List<DatabaseReference> References { get; set; } = new();
}

public class DatabaseField
{
    public string Name { get; set; }
    public string Type { get; set; }
    public bool IsNullable { get; set; }
    public int? MaxLength { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsTag { get; set; }
    public bool IsIndexed { get; set; }
}

public class DatabaseReference
{
    public string Field { get; set; }
    public string ReferencedTable { get; set; }
    public string ReferencedField { get; set; }
}
