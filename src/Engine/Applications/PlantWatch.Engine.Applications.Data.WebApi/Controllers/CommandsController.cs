using Microsoft.AspNetCore.Mvc;
using PlantWatch.Engine.Applications.Common.DTOs;
using PlantWatch.Engine.Applications.Data.WebApi.Mappers;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
using PlantWatch.Engine.Core.Data.Models.Schema;

namespace PlantWatch.Engine.Applications.Data.WebApi.Controllers;

[ApiController]
[Route("api/data/commands")]
public class CommandsController : ControllerBase
{
    private readonly IDatabaseDriverManager _driverManager;

    public CommandsController(IDatabaseDriverManager driverManager)
    {
        _driverManager = driverManager;
    }

    public record CommandRequest(Guid DriverId, string Command);

    [HttpGet("{driverId}/schema")]
    public async Task<IActionResult> GetSchema(Guid driverId)
    {
        var driver = _driverManager.GetDriver(driverId);
        if (driver == null)
        {
            return NotFound(new EngineOperationResult
            {
                Success = false,
                Message = "Driver not found."
            });
        }

        var result = await driver.GetSchemaAsync();

        if (!result.Success)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Failed to retrieve schema.",
                Data = result.ErrorMessage
            });
        }

        if (result.Data is not DatabaseSchema schema)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Invalid schema format returned by driver."
            });
        }

        var dto = SchemaDtoMapper.Map(schema);

        return Ok(new EngineOperationResult
        {
            Success = true,
            Message = "Schema retrieved successfully.",
            Data = dto
        });
    }



    [HttpPost("validate")]
    public IActionResult ValidateCommand([FromBody] CommandRequest request)
    {
        var driver = _driverManager.GetDriver(request.DriverId);
        if (driver == null)
        {
            return NotFound(new EngineOperationResult
            {
                Success = false,
                Message = "Driver not found."
            });
        }

        var isValid = driver.ValidateCommand(request.Command);
        return Ok(new EngineOperationResult
        {
            Success = true,
            Message = isValid ? "Command is valid." : "Command is invalid.",
            Data = new { IsValid = isValid }
        });
    }

    [HttpPost("execute")]
    public async Task<IActionResult> ExecuteCommand([FromBody] CommandRequest request)
    {
        var driver = _driverManager.GetDriver(request.DriverId);
        if (driver == null)
        {
            return NotFound(new EngineOperationResult
            {
                Success = false,
                Message = "Driver not found."
            });
        }

        var result = await driver.ExecuteAsync(request.Command);
        return Ok(new EngineOperationResult
        {
            Success = result.Success,
            Message = result.Success ? "Command executed successfully." : "Command execution failed.",
            Data = result.Data ?? result.ErrorMessage
        });
    }
}