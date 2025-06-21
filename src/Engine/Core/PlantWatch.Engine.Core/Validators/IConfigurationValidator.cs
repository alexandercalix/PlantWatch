using System;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Models.Definitions;

namespace PlantWatch.Engine.Core.Validators;

public interface IConfigurationValidator
{
    Task ValidatePlcDefinitionAsync(PlcConnectionDefinition plcDefinition);
    Task ValidateTagAsync(PlcTagDefinition tag);
}