using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using PlantWatch.Engine.Core.Interfaces;
using PlantWatch.Engine.Core.Models.Definitions;

using PlantWatch.Engine.Core.Validators;
using PlantWatch.Engine.Core.Factories;
using PlantWatch.Core.Interfaces.Engine.Services;

namespace PlantWatch.DriverRuntime.Tests;

public class DriverManagerTests
{
    [Fact]
    public async Task ReloadDriversAsync_ShouldLoadValidDriversAndSkipInvalids()
    {
        // Arrange
        var mockRepo = new Mock<IConfigurationRepository>();

        // Create valid PLC config
        var validPlcId = Guid.NewGuid();
        var validConfig = new PlcConnectionDefinition
        {
            Id = validPlcId,
            Name = "PLC_Siemens_1",
            DriverType = "Siemens",
            IpAddress = "192.168.0.10",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>()
        };

        // Create invalid PLC config (validation will fail)
        var invalidPlcId = Guid.NewGuid();
        var invalidConfig = new PlcConnectionDefinition
        {
            Id = invalidPlcId,
            Name = "PLC_Invalid",
            DriverType = "Siemens",
            IpAddress = "",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>()
        };

        // Create unknown driver type
        var unknownConfig = new PlcConnectionDefinition
        {
            Id = Guid.NewGuid(),
            Name = "PLC_Unknown",
            DriverType = "Modbus",
            IpAddress = "192.168.0.50",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>()
        };

        // Mock DB repository returning all configs
        mockRepo.Setup(r => r.LoadAllPlcConfigurationsAsync())
            .ReturnsAsync(new List<PlcConnectionDefinition> { validConfig, invalidConfig, unknownConfig });

        // Mock Siemens Factory & Validator
        var mockFactory = new Mock<IDriverFactory>();
        mockFactory.Setup(f => f.DriverType).Returns("Siemens");

        var mockDriver = new Mock<IPLCService>();
        mockDriver.Setup(d => d.Id).Returns(validPlcId);
        mockDriver.Setup(d => d.Name).Returns(validConfig.Name);
        mockDriver.Setup(d => d.StartAsync()).Returns(Task.CompletedTask);
        mockDriver.Setup(d => d.StopAsync()).Returns(Task.CompletedTask);

        mockFactory.Setup(f => f.CreateDriver(validConfig)).Returns(mockDriver.Object);

        var mockValidator = new Mock<IConfigurationValidator>();
        mockValidator.Setup(v => v.ValidatePlcDefinitionAsync(validConfig)).Returns(Task.CompletedTask);
        mockValidator.Setup(v => v.ValidatePlcDefinitionAsync(invalidConfig))
            .ThrowsAsync(new Exception("Invalid config"));

        // Create DriverManager
        var manager = new DriverManager(mockRepo.Object);
        manager.RegisterDriverFactory(mockFactory.Object, mockValidator.Object);

        // Act
        await manager.ReloadDriversAsync();

        // Assert
        var loadedDrivers = manager.GetAllDrivers().ToList();
        Assert.Single(loadedDrivers);
        Assert.Equal(validPlcId, loadedDrivers[0].Id);

        var driverById = manager.GetDriver(validPlcId);
        Assert.NotNull(driverById);
        Assert.Equal(validPlcId, driverById.Id);

        var nonExistingDriver = manager.GetDriver(Guid.NewGuid());
        Assert.Null(nonExistingDriver);

        // Verify mocks
        mockValidator.Verify(v => v.ValidatePlcDefinitionAsync(validConfig), Times.Once);
        mockValidator.Verify(v => v.ValidatePlcDefinitionAsync(invalidConfig), Times.Once);
        mockFactory.Verify(f => f.CreateDriver(validConfig), Times.Once);
        mockDriver.Verify(d => d.StartAsync(), Times.Once);
    }
}
