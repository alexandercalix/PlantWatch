using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface IDatabaseConfigurationRepository
{
    Task<IEnumerable<DatabaseConfig>> LoadAllDatabaseConfigurationsAsync();
    Task<DatabaseConfig?> GetDatabaseConfigurationAsync(Guid id);
    Task SaveDatabaseConfigurationAsync(DatabaseConfig config);
    Task DeleteDatabaseConfigurationAsync(Guid id);
}
