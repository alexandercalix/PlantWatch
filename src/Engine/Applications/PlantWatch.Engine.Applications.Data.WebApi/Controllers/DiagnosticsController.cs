using Microsoft.AspNetCore.Mvc;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Core.Data.Interfaces;

namespace PlantWatch.Engine.Applications.Data.WebApi.Controllers;

[ApiController]
[Route("api/data/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly IDatabaseDriverManager _driverManager;

    public DiagnosticsController(IDatabaseDriverManager driverManager)
    {
        _driverManager = driverManager;
    }

    [HttpGet]
    public IActionResult GetAllDiagnostics()
    {
        var diagnostics = _driverManager.GetDiagnostics();
        return Ok(new EngineOperationResult
        {
            Success = true,
            Message = "Diagnostics retrieved successfully.",
            Data = diagnostics
        });
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetDiagnostics(Guid id)
    {
        var diag = _driverManager.GetDiagnostics(id);
        if (diag == null)
        {
            return NotFound(new EngineOperationResult
            {
                Success = false,
                Message = $"Driver with ID '{id}' not found."
            });
        }

        return Ok(new EngineOperationResult
        {
            Success = true,
            Message = "Driver diagnostics retrieved successfully.",
            Data = diag
        });
    }

    [HttpGet("basic")]
    public IActionResult GetBasicDiagnostics()
    {
        var drivers = _driverManager.GetAvailableDrivers();
        return Ok(new EngineOperationResult
        {
            Success = true,
            Message = "Basic diagnostics retrieved successfully.",
            Data = new { AvailableDrivers = drivers }
        });
    }
}