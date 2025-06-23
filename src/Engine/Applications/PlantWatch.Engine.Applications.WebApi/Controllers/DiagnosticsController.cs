using System;
using Microsoft.AspNetCore.Mvc;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.Engine.Applications.WebApi.DTOs;

namespace PlantWatch.Engine.Applications.WebApi.Controllers;

[ApiController]
[Route("api/engine/diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly IDriverOrchestrator _orchestrator;

    public DiagnosticsController(IDriverOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpGet("plcs")]
    public async Task<IActionResult> GetAllPlcDiagnostics()
    {
        var diagnostics = await _orchestrator.GetDiagnosticsAsync();
        return Ok(diagnostics);
    }

    [HttpGet("plc/{plcId}")]
    public async Task<IActionResult> GetPlcDiagnostics(Guid plcId)
    {
        var diagnostic = await _orchestrator.GetDiagnosticsAsync(plcId);
        if (diagnostic == null)
            return NotFound();

        return Ok(diagnostic);
    }

    [HttpGet("basic")]
    public IActionResult GetBasicDiagnostics()
    {
        var diagnostics = _orchestrator.GetDiagnosticsAsync().Result;
        var basicDiagnostics = diagnostics.Select(d => new PlcConnection()
        {
            Id = d.Id,
            IsConnected = d.IsConnected
        }).ToList();

        return Ok(basicDiagnostics);
    }
}