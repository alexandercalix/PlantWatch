using System.Collections.Generic;


namespace PlantWatch.Engine.Drivers.Core.Descriptors;

public class TagParameterDescriptor
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string DataType { get; set; }
    public IEnumerable<string>? AllowedValues { get; set; }
    public bool IsRequired { get; set; }
    public string? ValidationRegex { get; set; }
}
