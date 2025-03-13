using System.Net;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.LearnerData.Api.Controllers;

// TODO: Remove this controller once dev work proper has started on this service.
[Route("home")]
[ApiVersion("1.0")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    [Route("HelloWorld")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public IActionResult HelloWorld()
    {
        return new OkObjectResult("Hello world!");
    }
}