using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Applications.Data.WebApi.Controllers;
using PlantWatch.Engine.Core.Data.Interfaces;
using System.Linq;

namespace PlantWatch.Engine.Tests.Application.Data.WebApi.Controllers;

public class DiagnosticsControllerTests
{
    private readonly Mock<IDatabaseDriverManager> _mockDriverManager;
    private readonly DiagnosticsController _controller;

    public DiagnosticsControllerTests()
    {
        _mockDriverManager = new Mock<IDatabaseDriverManager>();
        _controller = new DiagnosticsController(_mockDriverManager.Object);
    }

    #region GetAllDiagnostics Tests
    [Fact]
    public void TestDeserializationAndFirst()
    {
        var json = """
            [{"Id":"8b83c6c2-6e4e-4e86-b221-ecf76d73f234","Name":"Test Driver"}]
        """;

        var deserialized = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json)!;

        Assert.Single(deserialized);
        Assert.Equal("Test Driver", deserialized.First()["Name"].GetString());
    }
    [Fact]
    public void GetAllDiagnostics_ReturnsOk_WithDiagnostics()
    {
        // Arrange
        var mockDiagnostic = new Mock<IDatabaseDriverDiagnostics>();
        mockDiagnostic.SetupGet(d => d.Id).Returns(Guid.NewGuid());
        mockDiagnostic.SetupGet(d => d.Name).Returns("Test Driver");

        var diagnostics = new[] { mockDiagnostic.Object };

        _mockDriverManager.Setup(m => m.GetDiagnostics()).Returns(diagnostics);

        // Act
        var result = _controller.GetAllDiagnostics();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        // Deserialize and validate
        var json = JsonSerializer.Serialize(response.Data);
        List<Dictionary<string, JsonElement>> deserialized = JsonSerializer.Deserialize<List<Dictionary<string, JsonElement>>>(json)!;


        Assert.Single(deserialized);
        Assert.Equal("Test Driver", deserialized.First()["Name"].GetString());

    }

    #endregion

    #region GetDiagnostics Tests

    [Fact]
    public void GetDiagnostics_ReturnsOk_WhenDriverFound()
    {
        var driverId = Guid.NewGuid();
        var mockDiagnostic = new Mock<IDatabaseDriverDiagnostics>();
        mockDiagnostic.SetupGet(d => d.Id).Returns(driverId);
        mockDiagnostic.SetupGet(d => d.Name).Returns("Driver Found");

        var diagnostics = mockDiagnostic.Object;

        _mockDriverManager.Setup(m => m.GetDiagnostics(driverId)).Returns(diagnostics);

        var result = _controller.GetDiagnostics(driverId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(diagnostics, response.Data);
    }

    [Fact]
    public void GetDiagnostics_ReturnsNotFound_WhenDriverNotFound()
    {
        var driverId = Guid.NewGuid();

        _mockDriverManager.Setup(m => m.GetDiagnostics(driverId))
                          .Returns((IDatabaseDriverDiagnostics?)null);

        var result = _controller.GetDiagnostics(driverId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(notFoundResult.Value);
        Assert.False(response.Success);
        Assert.Contains("not found", response.Message, StringComparison.OrdinalIgnoreCase);
    }

    #endregion

    #region GetBasicDiagnostics Tests

    [Fact]
    public void GetBasicDiagnostics_ReturnsOk_WithAvailableDrivers()
    {
        var drivers = new[] { "AvailableDriver1", "AvailableDriver2" };
        _mockDriverManager.Setup(m => m.GetAvailableDrivers()).Returns(drivers);

        var result = _controller.GetBasicDiagnostics();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
        Assert.NotNull(response.Data);

        // Serialize and Deserialize Data Safely
        var json = JsonSerializer.Serialize(response.Data);
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        var driversFromResponse = JsonSerializer.Deserialize<IEnumerable<string>>(
            deserialized["AvailableDrivers"].GetRawText()
        )!;

        Assert.Equal(drivers, driversFromResponse);
    }

    #endregion
}
