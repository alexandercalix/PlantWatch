using System;
using PlantWatch.Engine.Drivers.Protocols.Siemens.Factories;

namespace PlantWatch.Engine.Drivers.Protocols.Siemens.Tests.Factories;

public class SiemensTagFactoryTests
{
    [Fact]
    public void Create_ValidInputs_ShouldBuildTagCorrectly()
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "Tag1";
        var datatype = "Int";
        var address = "DB1.DBW0";
        // Act
        var tag = SiemensTagFactory.Create(id, name, datatype, address);

        // Assert
        Assert.NotNull(tag);
        Assert.Equal(id, tag.Id);
        Assert.Equal(name, tag.Name);
        Assert.Equal(datatype, tag.Datatype);
        Assert.Equal(address, tag.Address);
        Assert.NotNull(tag.Item);
        Assert.False(tag.Disabled);
        Assert.False(tag.Quality);
    }

    [Theory]
    [InlineData("Unknown")]
    [InlineData("String")]
    public void Create_InvalidDatatype_ShouldThrow(string invalidDatatype)
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "TagX";
        var address = "DB1.DBW0";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            SiemensTagFactory.Create(id, name, invalidDatatype, address));
    }

    [Theory]
    [InlineData("DB1.DBW")]
    [InlineData("M100")]
    [InlineData("InvalidFormat")]
    public void Create_InvalidAddress_ShouldThrow(string invalidAddress)
    {
        // Arrange
        var id = Guid.NewGuid();
        var name = "TagY";
        var datatype = "Bool";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            SiemensTagFactory.Create(id, name, datatype, invalidAddress));
    }


}