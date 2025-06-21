
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Models.Definitions;

namespace PlantWatch.Engine.Core.Interfaces
{


    public interface IConfigurationRepository
    {
        // PLC Level
        Task<IEnumerable<PlcConnectionDefinition>> LoadAllPlcConfigurationsAsync();
        Task<PlcConnectionDefinition> GetPlcConfigurationAsync(Guid plcId);
        Task SavePlcConfigurationAsync(PlcConnectionDefinition config);
        Task DeletePlcConfigurationAsync(Guid plcId);

        // Tag Level (por PLC)
        Task<IEnumerable<PlcTagDefinition>> LoadTagsAsync(Guid plcId);
        Task<PlcTagDefinition> GetTagAsync(Guid plcId, Guid tagId);
        Task AddOrUpdateTagAsync(Guid plcId, PlcTagDefinition tag);
        Task DeleteTagAsync(Guid plcId, Guid tagId);
    }


}
