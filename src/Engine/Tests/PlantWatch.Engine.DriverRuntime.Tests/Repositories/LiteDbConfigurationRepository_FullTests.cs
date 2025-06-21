using System;
using PlantWatch.Engine.Core.Models.Definitions;
using PlantWatch.DriverRuntime.Repositories;

namespace PlantWatch.DriverRuntime.Tests.Repositories;

public class LiteDbConfigurationRepository_FullTests
{
    private string GenerateTempDbPath() =>
        Path.Combine(Path.GetTempPath(), $"PlantWatch_Test_{Guid.NewGuid()}.db");

    private PlcConnectionDefinition CreateSamplePlc(string plcName)
    {
        return new PlcConnectionDefinition
        {
            Id = Guid.NewGuid(),  // Nuevo campo ID
            Name = plcName,
            DriverType = "Siemens",
            IpAddress = "192.168.0.100",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>
            {
                new PlcTagDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag1",
                    Datatype = "Bool",
                    Address = "DB1.DBX0.0"
                },
                new PlcTagDefinition
                {
                    Id = Guid.NewGuid(),
                    Name = "Tag2",
                    Datatype = "Int",
                    Address = "DB1.DBW2"
                }
            }
        };
    }

    [Fact]
    public async Task FullRepository_CRUD_Test()
    {
        var dbPath = GenerateTempDbPath();
        var password = "test-password";

        try
        {
            var repo = new LiteDbConfigurationRepository(dbPath, password);
            var plc = CreateSamplePlc("PLC_Main");

            // Inicialmente vacía
            var allPlcs = await repo.LoadAllPlcConfigurationsAsync();
            Assert.Empty(allPlcs);

            // Guardar PLC
            await repo.SavePlcConfigurationAsync(plc);
            allPlcs = await repo.LoadAllPlcConfigurationsAsync();
            Assert.Single(allPlcs);

            // Obtener por ID
            var loadedPlc = await repo.GetPlcConfigurationAsync(plc.Id);
            Assert.NotNull(loadedPlc);
            Assert.Equal(plc.Name, loadedPlc.Name);
            Assert.Equal(2, loadedPlc.Tags.Count);

            // Leer tags por PLC (por ID ahora)
            var tags = await repo.LoadTagsAsync(plc.Id);
            Assert.Equal(2, tags.Count());

            // Obtener un tag específico
            var firstTag = loadedPlc.Tags.First();
            var loadedTag = await repo.GetTagAsync(plc.Id, firstTag.Id);
            Assert.NotNull(loadedTag);
            Assert.Equal(firstTag.Id, loadedTag.Id);

            // Agregar nuevo tag
            var newTag = new PlcTagDefinition
            {
                Id = Guid.NewGuid(),
                Name = "NewTag",
                Datatype = "Real",
                Address = "DB1.DBD10"
            };
            await repo.AddOrUpdateTagAsync(plc.Id, newTag);

            var updatedTags = await repo.LoadTagsAsync(plc.Id);
            Assert.Equal(3, updatedTags.Count());

            // Eliminar tag
            await repo.DeleteTagAsync(plc.Id, newTag.Id);
            var afterDeleteTags = await repo.LoadTagsAsync(plc.Id);
            Assert.Equal(2, afterDeleteTags.Count());

            // Eliminar todo el PLC
            await repo.DeletePlcConfigurationAsync(plc.Id);
            allPlcs = await repo.LoadAllPlcConfigurationsAsync();
            Assert.Empty(allPlcs);
        }
        finally
        {
            if (File.Exists(dbPath))
                File.Delete(dbPath);
        }
    }
}