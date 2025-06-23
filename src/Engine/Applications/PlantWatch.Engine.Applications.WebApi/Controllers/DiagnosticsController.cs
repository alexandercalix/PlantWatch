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
        try
        {
            var diagnostics = await _orchestrator.GetDiagnosticsAsync();
            return Ok(new EngineOperationResult()
            {
                Success = true,
                Message = "Diagnostics retrieved successfully",
                Data = diagnostics
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult() { Success = false, Message = "Error retrieving diagnostics", Data = ex.Message });
        }

    }

    [HttpGet("plc/{plcId}")]
    public async Task<IActionResult> GetPlcDiagnostics(Guid plcId)
    {
        try
        {
            var diagnostic = await _orchestrator.GetDiagnosticsAsync(plcId);
            if (diagnostic == null)
                return NotFound(new EngineOperationResult()
                {
                    Success = false,
                    Message = $"PLC with Id {plcId} not found.",
                    Data = null
                });

            return Ok(new EngineOperationResult()
            {
                Success = true,
                Message = "PLC diagnostic retrieved successfully",
                Data = diagnostic
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult()
            {
                Success = false,
                Message = "Error retrieving PLC diagnostic",
                Data = ex.Message
            });
        }
    }


    [HttpGet("basic")]
    public async Task<IActionResult> GetBasicDiagnostics()
    {
        try
        {
            var diagnostics = await _orchestrator.GetDiagnosticsAsync();
            var basicDiagnostics = diagnostics.Select(d => new PlcConnection()
            {
                Id = d.Id,
                IsConnected = d.IsConnected
            }).ToList();

            return Ok(new EngineOperationResult()
            {
                Success = true,
                Message = "Basic diagnostics retrieved successfully",
                Data = basicDiagnostics
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult()
            {
                Success = false,
                Message = "Error retrieving basic diagnostics",
                Data = ex.Message
            });
        }
    }

}