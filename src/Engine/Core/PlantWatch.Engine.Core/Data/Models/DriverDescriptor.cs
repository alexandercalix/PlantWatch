using System;
using System.Collections.Generic;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Core.Data.Models;

public class DriverDescriptor : IDatabaseDriverDescriptor
{
    public string DriverType { get; set; } = null!;
    public string FriendlyName { get; set; } = null!;
    public List<DriverParameterDefinition> Parameters { get; set; } = new();
}

public class DriverParameterDefinition
{
    public string Key { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string Type { get; set; } = "string"; // string, int, bool, password, etc.
    public bool IsRequired { get; set; }
    public string? Placeholder { get; set; }
    public string? Description { get; set; }
}
