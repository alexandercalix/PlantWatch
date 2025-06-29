using System.Collections.Generic;
using PlantWatch.Engine.Drivers.Core.Descriptors;

namespace PlantWatch.Engine.Drivers.Protocols.Siemens.Descriptors;

public class SiemensTagValidationDescriptor : ITagValidationDescriptor
{
    public string DriverType => "Siemens";

    public IEnumerable<TagParameterDescriptor> Parameters => new[]
    {
        new TagParameterDescriptor
        {
            Name = "Name", DisplayName = "Tag Name", DataType = "string", IsRequired = true
        },
        new TagParameterDescriptor
        {
            Name = "Datatype", DisplayName = "Datatype", DataType = "enum",
            AllowedValues = new[] { "Bool", "Byte", "Word", "DWord", "Int", "DInt", "Real" },
            IsRequired = true
        },
        new TagParameterDescriptor
        {
            Name = "Address", DisplayName = "Address", DataType = "string",
            ValidationRegex = @"^DB\d+\.DB(X|W|D)(\d+)(\.(\d+))?$|^[MIQ](B|W|D)(\d+)$|^[MIQ](\d+)\.(\d+)$",
            IsRequired = true
        }
    };

    public IEnumerable<string> SupportedDatatypes => new[]
    {
        "Bool", "Byte", "Word", "DWord", "Int", "DInt", "Real"
    };

    public string AddressPattern =>
        @"^DB\d+\.DB(X|W|D)(\d+)(\.(\d+))?$|^[MIQ](B|W|D)(\d+)$|^[MIQ](\d+)\.(\d+)$";

    public string AddressExample => "DB1.DBX0.0 / MB100 / Q2.1";
}