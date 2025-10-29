using Microsoft.AspNetCore.Mvc;

namespace DogsHouseService.Controllers;

[ApiController]
[Route("[controller]")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Dogshouseservice.Version1.0.1");
    }
}