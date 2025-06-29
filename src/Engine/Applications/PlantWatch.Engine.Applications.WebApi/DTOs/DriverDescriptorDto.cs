using System;

namespace PlantWatch.Engine.Applications.WebApi.DTOs;

public class DriverDescriptorDto
{
    public string DriverType { get; set; }
    public DriverCapabilitiesDto Capabilities { get; set; }
    public PlcConfigurationDescriptorDto PlcDescriptor { get; set; }
    public TagValidationDescriptorDto TagDescriptor { get; set; }
}

public class DriverCapabilitiesDto
{
    public string DriverType { get; set; }
    public IEnumerable<string> SupportedDeviceModels { get; set; }
    public string Description { get; set; }
}

public class PlcConfigurationDescriptorDto
{
    public IEnumerable<PlcParameterDescriptorDto> Parameters { get; set; }
}

public class PlcParameterDescriptorDto
{
    public string Name { get; set; }
    public string DisplayName { get; set; }  // ← agregar
    public string Type { get; set; }
    public bool Required { get; set; }
    public IEnumerable<string> AllowedValues { get; set; } = new List<string>();  // ← reemplazar EnumValues
}

public class TagValidationDescriptorDto
{
    public IEnumerable<string> SupportedDatatypes { get; set; }
    public string AddressPattern { get; set; }  // Regex que puede usar el frontend
    public string AddressExample { get; set; }  // Ejemplo para mostrar
}