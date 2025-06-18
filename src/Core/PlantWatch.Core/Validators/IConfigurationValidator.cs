using System;
using System.Threading.Tasks;
using PlantWatch.Core.Models.Definitions;

namespace PlantWatch.Core.Validators;

public interface IConfigurationValidator
{
    Task ValidatePlcDefinitionAsync(PlcConnectionDefinition plcDefinition);
    Task ValidateTagAsync(PlcTagDefinition tag);
}