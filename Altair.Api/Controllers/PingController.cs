using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Altair.Api.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping()
    {
        return NoContent();
    }
}
