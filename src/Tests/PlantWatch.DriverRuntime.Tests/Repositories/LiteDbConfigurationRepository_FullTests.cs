using System;
using PlantWatch.Core.Models.Definitions;
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

        try
        {
            var repo = new LiteDbConfigurationRepository(dbPath);

            var plc = CreateSamplePlc("PLC_Main");

            // Test: Initially empty
            var allPlcs = await repo.LoadAllPlcConfigurationsAsync();
            Assert.Empty(allPlcs);

            // Test: Save PLC
            await repo.SavePlcConfigurationAsync(plc);
            allPlcs = await repo.LoadAllPlcConfigurationsAsync();
            Assert.Single(allPlcs);

            // Test: Get PLC by name
            var loadedPlc = await repo.GetPlcConfigurationAsync(plc.Name);
            Assert.NotNull(loadedPlc);
            Assert.Equal(plc.Name, loadedPlc.Name);
            Assert.Equal(2, loadedPlc.Tags.Count);

            // Test: Load tags by PLC
            var tags = await repo.LoadTagsAsync(plc.Name);
            Assert.Equal(2, tags.Count());

            // Test: Get individual tag
            var firstTag = loadedPlc.Tags.First();
            var loadedTag = await repo.GetTagAsync(plc.Name, firstTag.Id);
            Assert.NotNull(loadedTag);
            Assert.Equal(firstTag.Id, loadedTag.Id);

            // Test: Add new Tag
            var newTag = new PlcTagDefinition
            {
                Id = Guid.NewGuid(),
                Name = "NewTag",
                Datatype = "Real",
                Address = "DB1.DBD10"
            };
            await repo.AddOrUpdateTagAsync(plc.Name, newTag);

            var updatedTags = await repo.LoadTagsAsync(plc.Name);
            Assert.Equal(3, updatedTags.Count());

            // Test: Delete tag
            await repo.DeleteTagAsync(plc.Name, newTag.Id);
            var afterDeleteTags = await repo.LoadTagsAsync(plc.Name);
            Assert.Equal(2, afterDeleteTags.Count());

            // Test: Delete entire PLC
            await repo.DeletePlcConfigurationAsync(plc.Name);
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