using System;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Tests.Data.MsSql;

public class MsSqlValidatorTests
{
    private readonly MsSqlValidator _validator = new MsSqlValidator();

    [Theory]
    [InlineData("SELECT * FROM Users")]
    [InlineData("  INSERT INTO table VALUES (1)")]
    [InlineData("\tUPDATE Table SET Column = 1")]
    [InlineData("\nDELETE FROM Table WHERE Id = 1")]
    public void Validate_ValidCommands_ReturnsTrue(string command)
    {
        var result = _validator.Validate(command);
        Assert.True(result);
    }

    [Theory]
    [InlineData("DROP TABLE Users")]
    [InlineData("CREATE TABLE Test")]
    [InlineData("ALTER TABLE Test")]
    [InlineData("TRUNCATE TABLE Test")]
    [InlineData("EXEC sp_who")]
    [InlineData("MERGE INTO Users")]
    public void Validate_UnsupportedCommands_ReturnsFalse(string command)
    {
        var result = _validator.Validate(command);
        Assert.False(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_EmptyOrNull_ReturnsFalse(string command)
    {
        var result = _validator.Validate(command);
        Assert.False(result);
    }

    [Fact]
    public void Validate_MixedCasing_Command_StillValid()
    {
        var result = _validator.Validate("   sElEct 1");
        Assert.True(result);
    }
}