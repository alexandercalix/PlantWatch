using System;
using Microsoft.AspNetCore.Mvc;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Data;
using PlantWatch.Engine.Applications.Common.DTOs;

namespace PlantWatch.Engine.Applications.Data.WebApi.Controllers;

[ApiController]
[Route("api/data/databases")]
public class DatabaseConfigurationController : ControllerBase
{
    private readonly IDatabaseDriverOrchestrator _orchestrator;

    public DatabaseConfigurationController(IDatabaseDriverOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDatabases()
    {
        try
        {
            var configs = await _orchestrator.GetAllDatabaseDiagnosticsAsync();
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Database configurations retrieved successfully.",
                Data = configs
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error retrieving database configurations.",
                Data = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateDatabase([FromBody] DatabaseConfig config)
    {
        try
        {
            await _orchestrator.CreateDatabaseAsync(config);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Database created successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error creating database.",
                Data = ex.Message
            });
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDatabase([FromBody] DatabaseConfig config)
    {
        try
        {
            await _orchestrator.UpdateDatabaseAsync(config);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Database updated successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error updating database.",
                Data = ex.Message
            });
        }
    }

    [HttpDelete("{databaseId}")]
    public async Task<IActionResult> DeleteDatabase(Guid databaseId)
    {
        try
        {
            await _orchestrator.DeleteDatabaseAsync(databaseId);
            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Database deleted successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error deleting database.",
                Data = ex.Message
            });
        }
    }
}
