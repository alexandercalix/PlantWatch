using System;
using Microsoft.AspNetCore.Mvc;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Applications.WebApi.DTOs;
using PlantWatch.Engine.Core.Models.Definitions;

namespace PlantWatch.Engine.Applications.WebApi.Controllers;

[ApiController]
[Route("api/engine/plcs")]
public class PlcConfigurationController : ControllerBase
{
    private readonly IDriverOrchestrator _orchestrator;

    public PlcConfigurationController(IDriverOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPlcs()
    {
        try
        {
            var plcs = await _orchestrator.GetDiagnosticsAsync();
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "PLC configurations retrieved successfully.",
                Data = plcs
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error retrieving PLC configurations.",
                Data = ex.Message
            });
        }
    }

    [HttpPost("plc")]
    public async Task<IActionResult> CreatePlc([FromBody] PlcConnectionDefinition plc)
    {
        try
        {
            await _orchestrator.CreatePlcAsync(plc);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "PLC created successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error creating PLC.",
                Data = ex.Message
            });
        }
    }

    [HttpPut("plc")]
    public async Task<IActionResult> UpdatePlc([FromBody] PlcConnectionDefinition plc)
    {
        try
        {
            await _orchestrator.UpdatePlcAsync(plc);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "PLC updated successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error updating PLC.",
                Data = ex.Message
            });
        }
    }

    [HttpDelete("plc/{plcId}")]
    public async Task<IActionResult> DeletePlc(Guid plcId)
    {
        try
        {
            await _orchestrator.DeletePlcAsync(plcId);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "PLC deleted successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error deleting PLC.",
                Data = ex.Message
            });
        }
    }

    [HttpPost("plc/{plcId}/tag")]
    public async Task<IActionResult> AddOrUpdateTag(Guid plcId, [FromBody] PlcTagDefinition tag)
    {
        try
        {
            await _orchestrator.AddOrUpdateTagAsync(plcId, tag);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Tag added/updated successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error adding/updating tag.",
                Data = ex.Message
            });
        }
    }

    [HttpDelete("plc/{plcId}/tag/{tagId}")]
    public async Task<IActionResult> DeleteTag(Guid plcId, Guid tagId)
    {
        try
        {
            await _orchestrator.DeleteTagAsync(plcId, tagId);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Tag deleted successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error deleting tag.",
                Data = ex.Message
            });
        }
    }
}