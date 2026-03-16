using System.Net;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Application.Queries.GetCourseCodesByUkprn;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;
using SFA.DAS.LearnerData.Application.Queries.GetSearch;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.Api.Controllers;

[Route("providers/{ukprn:long}/learners")]
[ApiVersion("1.0")]
[ApiController]
public class ProviderLearnersController(
    ISender sender,
    IPagedLinkHeaderService pagedLinkHeaderService,
    ILogger<ProviderLearnersController> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Search(
        long ukprn,
        [FromQuery] SearchLearnersRequest request)
    {
        var query = new GetSearchQuery(ukprn,request);

        var result = await sender.Send(query);

        var pageLinks = pagedLinkHeaderService.GetPageLinks(query, result);

        Response?.Headers.Add(pageLinks);

        return Ok(GetSearchResponse.MapFrom(result));
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [Route("{uln:long}")]
    public async Task<IActionResult> Save(long ukprn, long uln, [FromBody] SaveLearnerRequest request)
    {
        if (ukprn != request.Ukprn || uln != request.Uln || request.LearningType == null)
        {
            return BadRequest();
        }

        var command = new SaveLearnerNewCommand
        {
            Uln = request.Uln,
            Ukprn = request.Ukprn,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Dob = request.Dob,
            AcademicYear = request.AcademicYear,
            StartDate = request.StartDate,
            PlannedEndDate = request.PlannedEndDate,
            PercentageLearningToBeDelivered = request.PercentageLearningToBeDelivered,
            EpaoPrice = request.EpaoPrice,
            TrainingPrice = request.TrainingPrice,
            AgreementId = request.AgreementId,
            ConsumerReference = request.ConsumerReference,
            CorrelationId = request.CorrelationId,
            ReceivedDate = request.ReceivedDate,
            StandardCode = request.StandardCode,
            TrainingCode = request.LarsCode,
            TrainingName = request.TrainingName,
            LearningType = request.LearningType.Value,
            IsFlexiJob = request.IsFlexiJob,
            PlannedOTJTrainingHours = request.PlannedOTJTrainingHours
        };

        var response = await sender.Send(command);

        if (response.Result == SaveLearnerNewResult.Created)
        {
            return CreatedAtAction(nameof(GetById), new { ukprn, id = response.Id }, command);
        }

        var location = $"{Request?.Scheme}://{Request?.Host}/providers/{request.Ukprn}/learners/{response.Id}";
        Response?.Headers.Add(new KeyValuePair<string, StringValues>("location", location));

        return Ok();
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Route("{id:long}")]
    public async Task<IActionResult> GetById(long ukprn, long id)
    {
        var command = new GetLearnerByIdQuery(ukprn, id);

        var result = await sender.Send(command);

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

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("coursecodes")]
    public async Task<IActionResult> GetCourseCodesByUkprn(long ukprn)
    {
        var query = new GetCourseCodesByUkprnQuery(ukprn);

        var result = await sender.Send(query);

        return Ok(result);
    }
}