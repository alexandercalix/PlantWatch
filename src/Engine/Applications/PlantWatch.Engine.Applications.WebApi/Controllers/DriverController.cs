using System;
using Microsoft.AspNetCore.Mvc;
using PlantWatch.DriverRuntime;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Applications.WebApi.DTOs;
using PlantWatch.Engine.Applications.WebApi.Mappers;

namespace PlantWatch.Engine.Applications.WebApi.Controllers;

[ApiController]
[Route("api/engine/drivers")]
public class DriverController : ControllerBase
{

    private readonly IDriverManager _driverManager;

    public DriverController(IDriverManager driverManager)
    {
        _driverManager = driverManager ?? throw new ArgumentNullException(nameof(driverManager));
    }

    [HttpGet("descriptors")]
    public IActionResult GetDriverDescriptors()
    {
        var factories = _driverManager.GetAllFactories();

        var descriptors = factories.Select(factory => new DriverDescriptorDto
        {
            DriverType = factory.DriverType,
            Capabilities = DriverDescriptorMapper.MapCapabilities(factory.GetCapabilities()),
            PlcDescriptor = DriverDescriptorMapper.MapPlcDescriptor(factory.GetPlcDescriptor()),
            TagDescriptor = DriverDescriptorMapper.MapTagDescriptor(factory.GetTagDescriptor())
        }).ToList();

        return Ok(new EngineOperationResult
        {
            Success = true,
            Message = "Driver descriptors retrieved successfully",
            Data = descriptors
        });
    }


}
