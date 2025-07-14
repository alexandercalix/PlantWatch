using Microsoft.AspNetCore.Mvc;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Applications.Data.WebApi.Controllers;

[ApiController]
[Route("api/data/drivers")]
public class DriversController : ControllerBase
{
    private readonly IDatabaseDriverManager _driverManager;

    public DriversController(IDatabaseDriverManager driverManager)
    {
        _driverManager = driverManager;
    }

    [HttpGet("descriptors")]
    public IActionResult GetDriverDescriptors()
    {
        var descriptors = _driverManager.GetDriverDescriptors();
        return Ok(new EngineOperationResult
        {
            Success = true,
            Message = "Driver descriptors retrieved successfully.",
            Data = descriptors
        });
    }
}
