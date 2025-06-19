using System;
using PlantWatch.Core.Factories;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.Drivers.Siemens.Factories;

namespace PlantWatch.Drivers.Siemens.Tests.Factories;

public class SiemensPLCServiceFactoryTests
{
    private readonly IDriverFactory _factory;

    public SiemensPLCServiceFactoryTests()
    {
        _factory = new SiemensPLCServiceFactory();
    }

    [Fact]
    public void CreateDriver_WithValidConfig_ShouldCreatePLCService()
    {
        var config = BuildValidConfig();

        var plcService = _factory.CreateDriver(config);

        Assert.NotNull(plcService);
        Assert.Equal(config.Name, plcService.Name);
        Assert.Equal(config.Tags.Count, plcService.Tags.Count());
    }

    [Fact]
    public void CreateDriver_WithEmptyTags_ShouldCreatePLCServiceWithoutTags()
    {
        var config = new PlcConnectionDefinition
        {
            Id = new Guid(),
            Name = "PLC_Empty",
            DriverType = "Siemens",
            IpAddress = "192.168.0.10",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>() // empty list
        };

        var plcService = _factory.CreateDriver(config);

        Assert.NotNull(plcService);
        Assert.Empty(plcService.Tags);
    }

    [Fact]
    public void CreateDriver_WithInvalidDatatype_ShouldThrow()
    {
        var config = BuildValidConfig();
        config.Tags[0].Datatype = "INVALID_TYPE";

        Assert.Throws<ArgumentException>(() =>
        {
            _factory.CreateDriver(config);
        });
    }

    [Fact]
    public void CreateDriver_WithInvalidAddress_ShouldThrow()
    {
        var config = BuildValidConfig();
        config.Tags[0].Address = "XXX999";  // invalid address format

        Assert.Throws<ArgumentException>(() =>
        {
            _factory.CreateDriver(config);
        });
    }

    [Fact]
    public void CreateDriver_WithMissingAddress_ShouldThrow()
    {
        var config = BuildValidConfig();
        config.Tags[0].Address = "";  // empty address

        Assert.Throws<ArgumentException>(() =>
        {
            _factory.CreateDriver(config);
        });
    }



    private PlcConnectionDefinition BuildValidConfig()
    {
        return new PlcConnectionDefinition
        {
            Name = "TestPLC",
            DriverType = "Siemens",
            IpAddress = "192.168.0.1",
            Rack = 0,
            Slot = 1,
            Tags = new List<PlcTagDefinition>
                {
                    new PlcTagDefinition
                    {
                        Id = Guid.NewGuid(),
                        Name = "BoolTag",
                        Datatype = "Bool",
                        Address = "DB1.DBX0.0",
                        DefaultValue = true
                    },
                    new PlcTagDefinition
                    {
                        Id = Guid.NewGuid(),
                        Name = "IntTag",
                        Datatype = "Int",
                        Address = "DB1.DBW2",
                        DefaultValue = 42
                    }
                }
        };
    }
}