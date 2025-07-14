using System;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Applications.Data.WebApi.Controllers;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Tests.Application.Data.WebApi.Controllers;

public class DriversControllerTests
{
    private readonly Mock<IDatabaseDriverManager> _mockDriverManager;
    private readonly DriversController _controller;

    public DriversControllerTests()
    {
        _mockDriverManager = new Mock<IDatabaseDriverManager>();
        _controller = new DriversController(_mockDriverManager.Object);
    }

    [Fact]
    public void GetDriverDescriptors_ReturnsOk_WithDescriptors()
    {
        // Arrange
        var descriptors = new Dictionary<string, string>
        {
            { "sqlserver", "SQL Server Driver" },
            { "postgresql", "PostgreSQL Driver" }
        };

        _mockDriverManager.Setup(m => m.GetDriverDescriptors()).Returns(descriptors);

        // Act
        var result = _controller.GetDriverDescriptors();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<EngineOperationResult>(okResult.Value);
        Assert.True(response.Success);
        Assert.Equal(descriptors, response.Data);
    }
}
