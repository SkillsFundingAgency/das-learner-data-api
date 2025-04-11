using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.LearnerData.Api.Controllers;
[Route("api")]
[ApiController]
[AllowAnonymous]
public class InfoController(IConfiguration config) : ControllerBase
{
    [HttpGet("info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetInfo()
    {
        var info = new
        {
            Version = config["ApiVersion"] ?? "1.0.0",
            Name = "Learner Data API"
        };
        return Ok(info);
    }
}
