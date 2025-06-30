using System;

namespace PlantWatch.Engine.Core.Data.Models;

public class SqlColumnInfo
{
    public string Table { get; set; } = string.Empty;
    public string Column { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
}
