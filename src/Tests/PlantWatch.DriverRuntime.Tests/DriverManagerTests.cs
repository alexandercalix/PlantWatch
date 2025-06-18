using System;
using Moq;
using PlantWatch.Core.Factories;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.Core.Services.Drivers;
using PlantWatch.DriverRuntime.Interfaces;
using IDriverFactory = PlantWatch.DriverRuntime.Interfaces.IDriverFactory;

namespace PlantWatch.DriverRuntime.Tests;

public class DriverManagerTests
{
    [Fact]
    public async Task ReloadDriversAsync_ShouldLoadValidDriversAndSkipInvalids()
    {
        // Arrange
        var mockRepo = new Mock<IConfigurationRepository>();


        // Create a valid config (Siemens)
        var validConfig = new PlcConnectionDefinition
        {
            Name = "PLC_Siemens_1",
            DriverType = "Siemens",
            IpAddress = "192.168.0.10",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>()
        };

        // Create invalid config (Validator will fail)
        var invalidConfig = new PlcConnectionDefinition
        {
            Name = "PLC_Invalid",
            DriverType = "Siemens",
            IpAddress = "192.168.0.99",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>()
        };

        // Create unknown config (No factory registered)
        var unknownConfig = new PlcConnectionDefinition
        {
            Name = "PLC_Unknown",
            DriverType = "Modbus",
            IpAddress = "192.168.0.50",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>()
        };

        // Return all configs from repository
        mockRepo.Setup(r => r.LoadAllPlcConfigurationsAsync())
            .ReturnsAsync(new List<PlcConnectionDefinition> { validConfig, invalidConfig, unknownConfig });

        // Create mocks for Siemens Factory and Validator
        var mockFactory = new Mock<IDriverFactory>();
        mockFactory.Setup(f => f.DriverType).Returns("Siemens");

        var mockDriver = new Mock<IPLCService>();
        mockDriver.Setup(d => d.Name).Returns(validConfig.Name);
        mockDriver.Setup(d => d.StartAsync()).Returns(Task.CompletedTask);
        mockDriver.Setup(d => d.StopAsync()).Returns(Task.CompletedTask);

        mockFactory.Setup(f => f.CreateDriver(validConfig)).Returns(mockDriver.Object);

        var mockValidator = new Mock<IConfigurationValidator>();
        mockValidator.Setup(v => v.ValidatePlcDefinitionAsync(validConfig)).Returns(Task.CompletedTask);
        mockValidator.Setup(v => v.ValidatePlcDefinitionAsync(invalidConfig)).ThrowsAsync(new Exception("Invalid config"));

        // Create DriverManager
        var manager = new DriverManager(mockRepo.Object);
        manager.RegisterDriverFactory(mockFactory.Object, mockValidator.Object);

        // Act
        await manager.ReloadDriversAsync();

        // Assert
        var loadedDrivers = manager.GetAllDrivers().ToList();

        Assert.Single(loadedDrivers); // Only validConfig should succeed
        Assert.Equal("PLC_Siemens_1", loadedDrivers[0].Name);

        var driverByName = manager.GetDriver("PLC_Siemens_1");
        Assert.NotNull(driverByName);
        Assert.Equal("PLC_Siemens_1", driverByName.Name);

        var nonExistingDriver = manager.GetDriver("NonExistingPLC");
        Assert.Null(nonExistingDriver);

        // Verify mocks behavior
        mockValidator.Verify(v => v.ValidatePlcDefinitionAsync(validConfig), Times.Once);
        mockValidator.Verify(v => v.ValidatePlcDefinitionAsync(invalidConfig), Times.Once);
        mockFactory.Verify(f => f.CreateDriver(validConfig), Times.Once);
        mockDriver.Verify(d => d.StartAsync(), Times.Once);
    }
}
