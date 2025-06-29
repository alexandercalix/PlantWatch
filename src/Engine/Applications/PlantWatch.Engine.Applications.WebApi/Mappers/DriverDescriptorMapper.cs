using System;
using PlantWatch.Engine.Applications.WebApi.DTOs;
using PlantWatch.Engine.Core.Factories;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.Drivers.Core.Descriptors;

namespace PlantWatch.Engine.Applications.WebApi.Mappers;

public static class DriverDescriptorMapper
{
    public static DriverDescriptorDto Map(IDriverFactory factory)
    {
        return new DriverDescriptorDto
        {
            DriverType = factory.DriverType,
            Capabilities = MapCapabilities(factory.GetCapabilities()),
            PlcDescriptor = MapPlcDescriptor(factory.GetPlcDescriptor()),
            TagDescriptor = MapTagDescriptor(factory.GetTagDescriptor())
        };
    }


    public static PlcConfigurationDescriptorDto MapPlcDescriptor(IPlcConfigurationDescriptor descriptor)
    {
        return new PlcConfigurationDescriptorDto
        {
            Parameters = descriptor.Parameters.Select(p => new PlcParameterDescriptorDto
            {
                Name = p.Name,
                DisplayName = p.DisplayName,
                Type = p.DataType,
                Required = p.IsRequired,
                AllowedValues = p.AllowedValues?.ToList() ?? new List<string>()
            }).ToList()
        };
    }

    public static TagValidationDescriptorDto MapTagDescriptor(ITagValidationDescriptor descriptor)
    {
        return new TagValidationDescriptorDto
        {
            SupportedDatatypes = descriptor.SupportedDatatypes,
            AddressPattern = descriptor.AddressPattern,
            AddressExample = descriptor.AddressExample
        };
    }

    public static DriverCapabilitiesDto MapCapabilities(IDriverCapabilities capabilities)
    {
        return new DriverCapabilitiesDto
        {
            DriverType = capabilities.DriverType,
            SupportedDeviceModels = capabilities.SupportedDeviceModels,
            Description = capabilities.Description
        };
    }
}
