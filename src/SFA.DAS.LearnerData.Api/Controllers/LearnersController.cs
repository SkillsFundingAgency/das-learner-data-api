using System.Net;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries.GetAllLearners;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.Api.Controllers;

[Route("learners")]
[ApiVersion("1.0")]
[ApiController]
public class LearnersController(
    ISender sender,
    IPagedLinkHeaderService pagedLinkHeaderService,
    ILogger<LearnersController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAllLearners(
        [FromQuery] int page = 1,
        [FromQuery] int? pageSize = 100,
        [FromQuery] bool excludeApproved = true)
    {
        if(pageSize > 1000) return BadRequest();

        var query = new GetAllLearnersQuery(page, pageSize, excludeApproved);

        var result = await sender.Send(query);

        var pageLinks = pagedLinkHeaderService.GetPageLinks(query, result);

        Response?.Headers.Add(pageLinks);

        return Ok(GetAllLearnersResponse.MapFrom(result));
    }
}
