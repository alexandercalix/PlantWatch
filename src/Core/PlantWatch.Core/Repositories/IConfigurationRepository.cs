
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantWatch.Core.Models.Definitions;

namespace PlantWatch.Core.Interfaces
{
    public interface IConfigurationRepository
    {
        // PLC Level
        Task<IEnumerable<PlcConnectionDefinition>> LoadAllPlcConfigurationsAsync();
        Task<PlcConnectionDefinition> GetPlcConfigurationAsync(string plcName);
        Task SavePlcConfigurationAsync(PlcConnectionDefinition config);
        Task DeletePlcConfigurationAsync(string plcName);

        // Tag Level (por PLC)
        Task<IEnumerable<PlcTagDefinition>> LoadTagsAsync(string plcName);
        Task<PlcTagDefinition> GetTagAsync(string plcName, Guid tagId);
        Task AddOrUpdateTagAsync(string plcName, PlcTagDefinition tag);
        Task DeleteTagAsync(string plcName, Guid tagId);
    }

    public interface IConfigurationValidator
    {
        Task ValidatePlcDefinitionAsync(PlcConnectionDefinition plcDefinition);
        Task ValidateTagAsync(PlcTagDefinition tag);
    }


}
