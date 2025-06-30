using System;
using Moq;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Tests.Data.MsSql;

public class MsSqlDiagnosticsTests
{
    // Replace this with a valid local/test connection string
    private const string ValidConnectionString = "Server=localhost;Database=master;Trusted_Connection=True;";
    private const string InvalidConnectionString = "Server=invalidhost;Database=wrong;User Id=none;Password=none;";

    [Fact]
    public void IsOnline_WithValidConnection_ReturnsTrue()
    {
        var diagnostics = new MsSqlDiagnostics(ValidConnectionString);
        var result = diagnostics.IsOnline();
        Assert.True(result);
    }

    [Fact]
    public void IsOnline_WithInvalidConnection_ReturnsFalse()
    {
        var diagnostics = new MsSqlDiagnostics(InvalidConnectionString);
        var result = diagnostics.IsOnline();
        Assert.False(result);
    }


}