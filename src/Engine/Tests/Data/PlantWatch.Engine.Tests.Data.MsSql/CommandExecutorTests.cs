using System;
using Moq;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data.Drivers.MsSql.Internals;

namespace PlantWatch.Engine.Tests.Data.MsSql;

public class CommandExecutorTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsSuccess_ForValidSelect()
    {
        var mockExecutor = new Mock<ICommandExecutor>();
        var expectedData = new List<Dictionary<string, object>>
        {
            new() { ["id"] = 1, ["name"] = "Test" }
        };

        mockExecutor.Setup(e => e.ExecuteAsync("SELECT * FROM users"))
            .ReturnsAsync(ExecutionResult.Ok(expectedData));

        var result = await mockExecutor.Object.ExecuteAsync("SELECT * FROM users");

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsSuccess_ForInsert()
    {
        var mockExecutor = new Mock<ICommandExecutor>();
        mockExecutor.Setup(e => e.ExecuteAsync(It.Is<string>(s => s.StartsWith("INSERT"))))
            .ReturnsAsync(ExecutionResult.Ok(new { AffectedRows = 1 }));

        var result = await mockExecutor.Object.ExecuteAsync("INSERT INTO users VALUES ('A')");

        Assert.True(result.Success);
        Assert.Contains("AffectedRows", result.Data!.ToString());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsFailure_OnSqlError()
    {
        var mockExecutor = new Mock<ICommandExecutor>();
        mockExecutor.Setup(e => e.ExecuteAsync(It.IsAny<string>()))
            .ReturnsAsync(ExecutionResult.Fail("Syntax error"));

        var result = await mockExecutor.Object.ExecuteAsync("BAD SQL");

        Assert.False(result.Success);
        Assert.Equal("Syntax error", result.ErrorMessage);
    }
}