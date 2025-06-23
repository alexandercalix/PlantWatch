using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlantWatch.DriverRuntime;
using PlantWatch.DriverRuntime.Interfaces;
using PlantWatch.Engine.Applications.WebApi.DTOs;

namespace PlantWatch.Engine.Applications.WebApi.Controllers;

[ApiController]
[Route("api/engine/tags")]
public class TagsController : ControllerBase
{
    private readonly IDriverManager _driverManager;

    public TagsController(IDriverManager driverManager)
    {
        _driverManager = driverManager;
    }

    // GET: api/tags/full
    [HttpGet("full")]
    public IActionResult GetAllTagsFull()
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

        return Ok(tags);
    }

    // GET: api/tags/values
    [HttpGet("values")]
    public IActionResult GetAllTagValues()
    {
        var tags = _driverManager.GetAllDrivers()
            .SelectMany(plc => plc.Tags.Select(tag => new TagValueDto
            {
                Id = tag.Id,
                Value = tag.Value
            }))
            .ToList();

        return Ok(tags);
    }

    // POST: api/tags/values-by-ids
    [HttpPost("values-by-ids")]
    public IActionResult GetValuesByIds([FromBody] List<Guid> tagIds)
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

        return Ok(tags);
    }

    [HttpPost("write")]
    public async Task<IActionResult> WriteTagValue([FromBody] WriteTagRequest request)

    {

        var success = await _driverManager.WriteTagAsync(request.PlcId, request.TagId, request.Value);
        return success ? Ok() : BadRequest();
    }
}
