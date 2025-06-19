using System;
using Moq;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Models.Definitions;

namespace PlantWatch.DriverRuntime.Tests;

public class DriverOrchestratorTests
{
    private PlcConnectionDefinition CreateSamplePlc()
    {
        return new PlcConnectionDefinition
        {
            Id = Guid.NewGuid(),
            Name = "PLC_Test",
            DriverType = "Siemens",
            IpAddress = "192.168.0.10",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>()
        };
    }

    [Fact]
    public async Task FullWorkflow_CRUD_Should_Work_And_CallReload()
    {
        // Arrange
        var mockRepo = new Mock<IConfigurationRepository>();
        var mockDriverManager = new Mock<IDriverManager>();

        var plc = CreateSamplePlc();

        // The repo always returns existing PLCs when asked
        mockRepo.Setup(r => r.LoadAllPlcConfigurationsAsync())
            .ReturnsAsync(new List<PlcConnectionDefinition> { plc });

        // Create orchestrator
        var orchestrator = new DriverOrchestrator(mockRepo.Object, mockDriverManager.Object);

        // -------- PLC Creation --------
        var newPlc = CreateSamplePlc();
        newPlc.Id = Guid.Empty; // simulate that we're creating a new PLC

        await orchestrator.CreatePlcAsync(newPlc);

        mockRepo.Verify(r => r.SavePlcConfigurationAsync(It.Is<PlcConnectionDefinition>(p => p.Id != Guid.Empty)), Times.Once);
        mockDriverManager.Verify(m => m.ReloadDriversAsync(), Times.Once);

        // -------- PLC Update --------
        var existingPlc = CreateSamplePlc();
        existingPlc.Name = "UpdatedName";

        await orchestrator.UpdatePlcAsync(existingPlc);

        mockRepo.Verify(r => r.SavePlcConfigurationAsync(existingPlc), Times.Once);
        mockDriverManager.Verify(m => m.ReloadDriversAsync(), Times.Exactly(2));

        // -------- PLC Deletion --------
        mockRepo.Setup(r => r.LoadAllPlcConfigurationsAsync())
            .ReturnsAsync(new List<PlcConnectionDefinition> { plc });

        await orchestrator.DeletePlcAsync(plc.Id);

        mockRepo.Verify(r => r.DeletePlcConfigurationAsync(plc.Id), Times.Once);
        mockDriverManager.Verify(m => m.ReloadDriversAsync(), Times.Exactly(3));

        // -------- Add Tag --------
        var tag = new PlcTagDefinition
        {
            Id = Guid.Empty,
            Name = "Tag1",
            Datatype = "Real",
            Address = "DB1.DBD4"
        };

        await orchestrator.AddOrUpdateTagAsync(plc.Id, tag);

        mockRepo.Verify(r => r.AddOrUpdateTagAsync(plc.Id, It.Is<PlcTagDefinition>(t => t.Id != Guid.Empty)), Times.Once);
        mockDriverManager.Verify(m => m.ReloadDriversAsync(), Times.Exactly(4));

        // -------- Update Tag --------
        var tagToUpdate = new PlcTagDefinition
        {
            Id = Guid.NewGuid(),
            Name = "UpdatedTag",
            Datatype = "Int",
            Address = "DB1.DBW2"
        };

        await orchestrator.AddOrUpdateTagAsync(plc.Id, tagToUpdate);
        mockRepo.Verify(r => r.AddOrUpdateTagAsync(plc.Id, tagToUpdate), Times.Once);
        mockDriverManager.Verify(m => m.ReloadDriversAsync(), Times.Exactly(5));

        // -------- Delete Tag --------
        await orchestrator.DeleteTagAsync(plc.Id, tagToUpdate.Id);
        mockRepo.Verify(r => r.DeleteTagAsync(plc.Id, tagToUpdate.Id), Times.Once);
        mockDriverManager.Verify(m => m.ReloadDriversAsync(), Times.Exactly(6));
    }

    [Fact]
    public async Task DeletePlcAsync_ShouldNotThrow_WhenPlcDoesNotExist()
    {
        // Arrange
        var mockRepo = new Mock<IConfigurationRepository>();
        var mockDriverManager = new Mock<IDriverManager>();

        mockRepo.Setup(r => r.LoadAllPlcConfigurationsAsync())
            .ReturnsAsync(new List<PlcConnectionDefinition>());

        var orchestrator = new DriverOrchestrator(mockRepo.Object, mockDriverManager.Object);

        await orchestrator.DeletePlcAsync(Guid.NewGuid());

        // Should not throw, but should not attempt deletion
        mockRepo.Verify(r => r.DeletePlcConfigurationAsync(It.IsAny<Guid>()), Times.Never);
        mockDriverManager.Verify(m => m.ReloadDriversAsync(), Times.Never);
    }
}
