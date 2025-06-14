using System;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.Drivers.Siemens.Factories;

namespace PlantWatch.Drivers.Siemens.Tests.Factories;

public class SiemensPLCServiceFactoryTests
{
    [Fact]
    public void CreateFromConfig_ValidDefinition_ShouldCreateService()
    {
        // Arrange
        var config = new PlcConnectionDefinition
        {
            Name = "TestPLC",
            IpAddress = "192.168.0.1",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>
            {
                new PlcTagDefinition
                {
                    Name = "Tag1",
                    Datatype = "Bool",
                    Address = "DB1.DBX0.0",
                    DefaultValue = true
                },
                new PlcTagDefinition
                {
                    Name = "Tag2",
                    Datatype = "Int",
                    Address = "DB1.DBW2",
                    DefaultValue = 42
                }
            }
        };

        // Act
        var service = SiemensPLCServiceFactory.CreateFromConfig(config);

        // Assert
        Assert.NotNull(service);
        Assert.Equal("TestPLC", service.Name);
        Assert.Equal(2, service.Tags.Count());
    }

    [Fact]
    public void CreateFromConfig_InvalidTag_ThrowsException()
    {
        var config = new PlcConnectionDefinition
        {
            Name = "FaultyPLC",
            IpAddress = "192.168.0.2",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>
            {
                new PlcTagDefinition
                {
                    Name = "InvalidTag",
                    Datatype = "FloatyFloat",
                    Address = "DB1.DBD4",
                    DefaultValue = 3.14f
                }
            }
        };

        Assert.Throws<ArgumentException>(() =>
            SiemensPLCServiceFactory.CreateFromConfig(config));
    }
}