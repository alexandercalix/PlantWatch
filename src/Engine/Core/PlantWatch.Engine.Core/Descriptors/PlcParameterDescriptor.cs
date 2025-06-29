using System.Collections.Generic;


namespace PlantWatch.Engine.Drivers.Core.Descriptors;

public class PlcParameterDescriptor
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string DataType { get; set; } // Ej: "string", "int", "enum"
    public IEnumerable<string>? AllowedValues { get; set; } // si es enum
    public bool IsRequired { get; set; }
}
