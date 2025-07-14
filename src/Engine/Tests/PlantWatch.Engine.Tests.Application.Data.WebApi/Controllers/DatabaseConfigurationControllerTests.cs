using System;
using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using global::PlantWatch.Engine.Data;
using global::PlantWatch.Engine.Applications.Data.WebApi.Controllers;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Tests.Application.Data.WebApi.Controllers;

public class DatabaseConfigurationControllerTests
{
    private readonly Mock<IDatabaseDriverOrchestrator> _mockOrchestrator;
    private readonly DatabaseConfigurationController _controller;

    public DatabaseConfigurationControllerTests()
    {
        _mockOrchestrator = new Mock<IDatabaseDriverOrchestrator>();
        _controller = new DatabaseConfigurationController(_mockOrchestrator.Object);
    }

    #region POST Tests - Create Database

    [Fact]
    public async Task CreateDatabase_ReturnsOk_OnSuccess()
    {
        var config = new DatabaseConfig { Id = Guid.NewGuid(), Name = "Test DB" };

        _mockOrchestrator
            .Setup(o => o.CreateDatabaseAsync(config))
            .Returns(Task.CompletedTask);

        var result = await _controller.CreateDatabase(config);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task CreateDatabase_ReturnsInternalServerError_OnException()
    {
        var config = new DatabaseConfig { Id = Guid.NewGuid(), Name = "Test DB" };

        _mockOrchestrator
            .Setup(o => o.CreateDatabaseAsync(config))
            .ThrowsAsync(new Exception("Create failed"));

        var result = await _controller.CreateDatabase(config);

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, errorResult.StatusCode);
        var response = Assert.IsType<EngineOperationResult>(errorResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Create failed", response.Data.ToString());
    }

    #endregion

    #region PUT Tests - Update Database

    [Fact]
    public async Task UpdateDatabase_ReturnsOk_OnSuccess()
    {
        var config = new DatabaseConfig { Id = Guid.NewGuid(), Name = "Test DB" };

        _mockOrchestrator
            .Setup(o => o.UpdateDatabaseAsync(config))
            .Returns(Task.CompletedTask);

        var result = await _controller.UpdateDatabase(config);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task UpdateDatabase_ReturnsInternalServerError_OnException()
    {
        var config = new DatabaseConfig { Id = Guid.NewGuid(), Name = "Test DB" };

        _mockOrchestrator
            .Setup(o => o.UpdateDatabaseAsync(config))
            .ThrowsAsync(new Exception("Update failed"));

        var result = await _controller.UpdateDatabase(config);

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, errorResult.StatusCode);
        var response = Assert.IsType<EngineOperationResult>(errorResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Update failed", response.Data.ToString());
    }

    #endregion

    #region DELETE Tests - Delete Database

    [Fact]
    public async Task DeleteDatabase_ReturnsOk_OnSuccess()
    {
        var id = Guid.NewGuid();

        _mockOrchestrator
            .Setup(o => o.DeleteDatabaseAsync(id))
            .Returns(Task.CompletedTask);

        var result = await _controller.DeleteDatabase(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
    }

    [Fact]
    public async Task DeleteDatabase_ReturnsInternalServerError_OnException()
    {
        var id = Guid.NewGuid();

        _mockOrchestrator
            .Setup(o => o.DeleteDatabaseAsync(id))
            .ThrowsAsync(new Exception("Delete failed"));

        var result = await _controller.DeleteDatabase(id);

        var errorResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, errorResult.StatusCode);
        var response = Assert.IsType<EngineOperationResult>(errorResult.Value);
        Assert.False(response.Success);
        Assert.Contains("Delete failed", response.Data.ToString());
    }

    #endregion
}