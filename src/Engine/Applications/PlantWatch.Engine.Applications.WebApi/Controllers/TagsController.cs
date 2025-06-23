using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlantWatch.DriverRuntime;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.Engine.Applications.WebApi.DTOs;

namespace PlantWatch.Engine.Applications.WebApi.Controllers;

// Controller to expose tag runtime operations
[ApiController]
[Route("api/engine/tags")]
public class TagsController : ControllerBase
{
    private readonly IDriverManager _driverManager;

    public TagsController(IDriverManager driverManager)
    {
        _driverManager = driverManager;
    }

    /// <summary>
    /// Get full information of all tags loaded in runtime (includes PLC Id, Tag Id, Name, Datatype, Value, and Quality).
    /// </summary>
    [HttpGet("full")]
    public IActionResult GetAllTagsFull()
    {
        try
        {
            var tags = _driverManager.GetAllDrivers()
                .SelectMany(plc => plc.Tags.Select(tag => new TagFullDto
                {
                    Id = plc.Id,
                    TagId = tag.Id,
                    Name = tag.Name,
                    Datatype = tag.Datatype,
                    Value = tag.Value,
                    Quality = tag.Quality
                }))
                .ToList();

            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "All tags retrieved successfully.",
                Data = tags
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error retrieving tags.",
                Data = ex.Message
            });
        }
    }

    /// <summary>
    /// Get current values of all tags (only Tag Id and Value).
    /// Useful for periodic polling or dashboards.
    /// </summary>
    [HttpGet("values")]
    public IActionResult GetAllTagValues()
    {
        try
        {
            var tags = _driverManager.GetAllDrivers()
                .SelectMany(plc => plc.Tags.Select(tag => new TagValueDto
                {
                    Id = tag.Id,
                    Value = tag.Value
                }))
                .ToList();

            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Tag values retrieved successfully.",
                Data = tags
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error retrieving tag values.",
                Data = ex.Message
            });
        }
    }

    /// <summary>
    /// Get values for a specific list of tag Ids.
    /// </summary>
    [HttpPost("values-by-ids")]
    public IActionResult GetValuesByIds([FromBody] List<Guid> tagIds)
    {
        try
        {
            var tags = _driverManager.GetAllDrivers()
                .SelectMany(plc => plc.Tags)
                .Where(tag => tagIds.Contains(tag.Id))
                .Select(tag => new TagValueDto
                {
                    Id = tag.Id,
                    Value = tag.Value
                })
                .ToList();

            return Ok(new EngineOperationResult
            {
                Success = true,
                Message = "Tag values by IDs retrieved successfully.",
                Data = tags
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error retrieving tag values by IDs.",
                Data = ex.Message
            });
        }
    }

    /// <summary>
    /// Write a value to a specific tag by PLC Id and Tag Id.
    /// </summary>
    [HttpPost("write")]
    public async Task<IActionResult> WriteTagValue([FromBody] WriteTagRequest request)
    {
        try
        {
            var success = await _driverManager.WriteTagAsync(request.PlcId, request.TagId, request.Value);
            if (success)
            {
                return Ok(new EngineOperationResult
                {
                    Success = true,
                    Message = "Tag value written successfully."
                });
            }
            else
            {
                return BadRequest(new EngineOperationResult
                {
                    Success = false,
                    Message = "Failed to write tag value. Tag may be disabled or invalid."
                });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new EngineOperationResult
            {
                Success = false,
                Message = "Error writing tag value.",
                Data = ex.Message
            });
        }
    }
}
