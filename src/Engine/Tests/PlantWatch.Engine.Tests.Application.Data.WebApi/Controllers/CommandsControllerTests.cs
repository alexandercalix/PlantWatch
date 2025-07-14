using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Applications.Data.WebApi.Controllers;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Tests.Application.Data.WebApi.Controllers;

public class CommandsControllerTests
{
    private readonly Mock<IDatabaseDriverManager> _mockDriverManager;
    private readonly CommandsController _controller;

    public CommandsControllerTests()
    {
        _mockDriverManager = new Mock<IDatabaseDriverManager>();
        _controller = new CommandsController(_mockDriverManager.Object);
    }

    #region Validate Tests

    [Fact]
    public void ValidateCommand_ReturnsOk_WhenCommandIsValid()
    {
        var driverId = Guid.NewGuid();
        var mockDriver = new Mock<IDatabaseDriver>();
        mockDriver.Setup(d => d.ValidateCommand("valid")).Returns(true);

        _mockDriverManager.Setup(m => m.GetDriver(driverId)).Returns(mockDriver.Object);

        var request = new CommandsController.CommandRequest(driverId, "valid");

        var result = _controller.ValidateCommand(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("valid", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateCommand_ReturnsOk_WhenCommandIsInvalid()
    {
        var driverId = Guid.NewGuid();
        var mockDriver = new Mock<IDatabaseDriver>();
        mockDriver.Setup(d => d.ValidateCommand("invalid")).Returns(false);

        _mockDriverManager.Setup(m => m.GetDriver(driverId)).Returns(mockDriver.Object);

        var request = new CommandsController.CommandRequest(driverId, "invalid");

        var result = _controller.ValidateCommand(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("invalid", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ValidateCommand_ReturnsNotFound_WhenDriverNotFound()
    {
        var driverId = Guid.NewGuid();
        _mockDriverManager.Setup(m => m.GetDriver(driverId)).Returns((IDatabaseDriver?)null);

        var request = new CommandsController.CommandRequest(driverId, "any");

        var result = _controller.ValidateCommand(request);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("not found", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region Execute Tests

    [Fact]
    public async Task ExecuteCommand_ReturnsOk_WhenExecutionSucceeds()
    {
        var driverId = Guid.NewGuid();
        var mockDriver = new Mock<IDatabaseDriver>();
        mockDriver.Setup(d => d.ExecuteAsync("run"))
                  .ReturnsAsync(ExecutionResult.Ok("OK"));

        _mockDriverManager.Setup(m => m.GetDriver(driverId)).Returns(mockDriver.Object);

        var request = new CommandsController.CommandRequest(driverId, "run");

        var result = await _controller.ExecuteCommand(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
        Assert.Contains("successfully", response.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("OK", response.Data);
    }

    [Fact]
    public async Task ExecuteCommand_ReturnsOk_WhenExecutionFails()
    {
        var driverId = Guid.NewGuid();
        var mockDriver = new Mock<IDatabaseDriver>();
        mockDriver.Setup(d => d.ExecuteAsync("fail"))
                  .ReturnsAsync(ExecutionResult.Fail("Failed to execute"));

        _mockDriverManager.Setup(m => m.GetDriver(driverId)).Returns(mockDriver.Object);

        var request = new CommandsController.CommandRequest(driverId, "fail");

        var result = await _controller.ExecuteCommand(request);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.False(response.Success);
        Assert.Contains("failed", response.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("Failed to execute", response.Data);
    }


    [Fact]
    public async Task ExecuteCommand_ReturnsNotFound_WhenDriverNotFound()
    {
        var driverId = Guid.NewGuid();
        _mockDriverManager.Setup(m => m.GetDriver(driverId)).Returns((IDatabaseDriver?)null);

        var request = new CommandsController.CommandRequest(driverId, "any");

        var result = await _controller.ExecuteCommand(request);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("not found", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion
}