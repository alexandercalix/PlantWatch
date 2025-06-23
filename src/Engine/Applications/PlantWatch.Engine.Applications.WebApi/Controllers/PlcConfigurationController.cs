using System;
using Microsoft.AspNetCore.Mvc;
using PlantWatch.DriverRuntime.Interfaces;
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
        var plcs = await _orchestrator.GetDiagnosticsAsync();
        return Ok(plcs);
    }

    [HttpPost("plc")]
    public async Task<IActionResult> CreatePlc([FromBody] PlcConnectionDefinition plc)
    {
        await _orchestrator.CreatePlcAsync(plc);
        return Ok();
    }

    [HttpPut("plc")]
    public async Task<IActionResult> UpdatePlc([FromBody] PlcConnectionDefinition plc)
    {
        await _orchestrator.UpdatePlcAsync(plc);
        return Ok();
    }

    [HttpDelete("plc/{plcId}")]
    public async Task<IActionResult> DeletePlc(Guid plcId)
    {
        await _orchestrator.DeletePlcAsync(plcId);
        return Ok();
    }

    [HttpPost("plc/{plcId}/tag")]
    public async Task<IActionResult> AddOrUpdateTag(Guid plcId, [FromBody] PlcTagDefinition tag)
    {
        await _orchestrator.AddOrUpdateTagAsync(plcId, tag);
        return Ok();
    }

    [HttpDelete("plc/{plcId}/tag/{tagId}")]
    public async Task<IActionResult> DeleteTag(Guid plcId, Guid tagId)
    {
        await _orchestrator.DeleteTagAsync(plcId, tagId);
        return Ok();
    }
}