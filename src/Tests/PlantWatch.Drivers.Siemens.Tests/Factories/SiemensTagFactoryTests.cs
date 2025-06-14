using System;
using PlantWatch.Drivers.Siemens.Factories;

namespace PlantWatch.Drivers.Siemens.Tests.Factories;

public class SiemensTagFactoryTests
{
    [Fact]
    public void Create_ValidInputs_ShouldBuildTagCorrectly()
    {
        // Arrange
        var name = "Tag1";
        var datatype = "Int";
        var address = "DB1.DBW0";
        var defaultValue = 123;

        // Act
        var tag = SiemensTagFactory.Create(name, datatype, address, defaultValue);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(name, tag.Name);
        Assert.Equal(datatype, tag.Datatype);
        Assert.Equal(address, tag.Address);
        Assert.Equal(defaultValue, tag.Value);
        Assert.NotNull(tag.Item);
        Assert.Equal(123, tag.Item.Value);
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("String")]
    public void Create_InvalidDatatype_ShouldThrow(string invalidDatatype)
    {
        // Arrange
        var name = "TagX";
        var address = "DB1.DBW0";
        var value = 0;

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            SiemensTagFactory.Create(name, invalidDatatype, address, value));
    }
}