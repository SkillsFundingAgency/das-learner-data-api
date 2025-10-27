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

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Route("{id:long}")]
    public async Task<IActionResult> GetById(long ukprn, long id)
    {
        var command = new GetLearnerByIdQuery(ukprn, id);
        logger.LogInformation($"Get learner data from API for {id}");

        var result = await sender.Send(command);
        logger.LogInformation($"Learener data for {id} issucessful :{result.Found}");

        if (!result.Found)
        {
            return new NotFoundResult();
        }

        return Ok(GetLearnerByIdResponse.MapFrom(result));
    }

    [HttpPatch]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("{id:long}/apprenticeshipId")]
    public async Task<IActionResult> PatchApprenticeshipId(long ukprn, long id, [FromBody] PatchLearnerDataApprenticeshipIdRequest request)
    {
        try
        {
            var command = new AssignApprenticeshipIdCommand
            {
                Ukprn = ukprn,
                LearnerDataId = id,
                ApprenticeshipId = request.ApprenticeshipId,
            };

            await sender.Send(command);
        }
        catch (KeyNotFoundException ex)
        {
            logger.LogError(ex, ex.Message);
            return NotFound();

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error trying to assign apprenticeship Id {0} to Learner Data record {1}", request.ApprenticeshipId, id);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }
}
