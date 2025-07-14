using System;

namespace PlantWatch.Engine.Applications.Data.WebApi.DTOs;

public class DataStructureDto
{
    public string EntityName { get; set; }              // Table, Collection, Measurement
    public string EntityType { get; set; }              // "table", "collection", "measurement"
    public List<FieldDescriptorDto> Fields { get; set; } = new();
    public List<ReferenceDescriptorDto> References { get; set; } = new(); // Optional
}

public class FieldDescriptorDto
{
    public string FieldName { get; set; }
    public string DataType { get; set; }                // string, int, float, date, object, array...
    public bool IsNullable { get; set; }                // True for SQL NULL / optional fields
    public int? MaxLength { get; set; }                 // For strings
    public int? Precision { get; set; }                 // For decimal
    public int? Scale { get; set; }                     // For decimal
    public bool IsPrimaryKey { get; set; }              // SQL only
    public bool IsTag { get; set; }                     // InfluxDB: distinguish tag vs field
    public bool IsIndexed { get; set; }                 // MongoDB: allow future UI optimization
}

public class ReferenceDescriptorDto
{
    public string SourceField { get; set; }             // Orders.CustomerId
    public string TargetEntity { get; set; }            // Customers
    public string TargetField { get; set; }             // Customers.CustomerId
}
