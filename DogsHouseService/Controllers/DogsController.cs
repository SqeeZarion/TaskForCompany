using DogsHouseService.Application.DTOs;
using DogsHouseService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DogsHouseService.Controllers;

[ApiController]
[Route("[controller]")]
public class DogsController : ControllerBase
{
    private readonly IDogService _dogService;

    public DogsController(IDogService dogService)
    {
        _dogService = dogService;
    }

    // GET /dogs?attribute=weight&order=desc&pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetDogs(
        [FromQuery] string? attribute,
        [FromQuery] string? order,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var dogs = await _dogService.GetAllAsync(attribute, order, pageNumber, pageSize, cancellationToken);
        return Ok(dogs);
    }

    // POST /dog
    [HttpPost("/dog")]
    public async Task<IActionResult> CreateDog([FromBody] CreateDogRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dogService.CreateAsync(request, cancellationToken);
            return Created($"/dog/{result.Name}", result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}