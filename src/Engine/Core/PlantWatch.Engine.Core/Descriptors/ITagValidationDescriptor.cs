using System.Collections.Generic;


namespace PlantWatch.Engine.Drivers.Core.Descriptors;

public interface ITagValidationDescriptor
{
    string DriverType { get; }
    IEnumerable<TagParameterDescriptor> Parameters { get; }

    IEnumerable<string> SupportedDatatypes { get; }
    string AddressPattern { get; }
    string AddressExample { get; }
}
