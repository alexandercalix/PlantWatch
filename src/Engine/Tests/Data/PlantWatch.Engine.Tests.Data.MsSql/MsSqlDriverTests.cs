using System;
using System.Threading.Tasks;
using Moq;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Drivers.MsSql;

namespace PlantWatch.Engine.Tests.Data.MsSql;

public class MsSqlDriverTests
{
    private readonly Mock<ICommandValidator> _mockValidator = new();
    private readonly Mock<ICommandExecutor> _mockExecutor = new();
    private readonly Mock<IDatabaseDriverDiagnostics> _mockDiagnostics = new();

    private readonly Guid _driverId = Guid.NewGuid();

    private MsSqlDriver CreateDriver(string name = "mssql")
    {
        return new MsSqlDriver(
            id: _driverId,
            name: name,
            validator: _mockValidator.Object,
            executor: _mockExecutor.Object,
            diagnostics: _mockDiagnostics.Object
        );
    }

    [Fact]
    public void GetDescriptor_ReturnsExpectedValue()
    {
        var driver = CreateDriver();
        Assert.Equal("Microsoft SQL Server Driver (Split Core)", driver.GetDescriptor());
    }

    [Fact]
    public void ValidateCommand_ReturnsTrue_WhenValidatorReturnsTrue()
    {
        _mockValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(true);
        var driver = CreateDriver();

        var result = driver.ValidateCommand("SELECT * FROM users");

        Assert.True(result);
    }

    [Fact]
    public void ValidateCommand_ReturnsFalse_WhenValidatorReturnsFalse()
    {
        _mockValidator.Setup(v => v.Validate(It.IsAny<string>())).Returns(false);
        var driver = CreateDriver();

        var result = driver.ValidateCommand("DROP TABLE sensitive_data");

        Assert.False(result);
    }

    [Fact]
    public async Task IsOnline_ReturnsTrue_WhenDiagnosticsSaysOnline()
    {
        _mockDiagnostics.Setup(d => d.IsOnline).Returns(true);
        var driver = CreateDriver();

        Assert.True(await driver.IsOnline());
    }

    [Fact]
    public async Task IsOnline_ReturnsFalse_WhenDiagnosticsFails()
    {
        _mockDiagnostics.Setup(d => d.IsOnline).Returns(false);
        var driver = CreateDriver();

        Assert.False(await driver.IsOnline());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsExpectedResult()
    {
        var expected = ExecutionResult.Ok(new { AffectedRows = 1 });
        _mockExecutor.Setup(e => e.ExecuteAsync(It.IsAny<string>())).ReturnsAsync(expected);

        var driver = CreateDriver();
        var result = await driver.ExecuteAsync("INSERT INTO users (name) VALUES ('test')");

        Assert.True(result.Success);
        Assert.Equal(1, ((dynamic)result.Data).AffectedRows);
    }

    [Fact]
    public void Driver_Id_IsCorrect()
    {
        var driver = CreateDriver();
        Assert.Equal(_driverId, driver.Id);
    }
}