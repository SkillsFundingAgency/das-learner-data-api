using System.Net;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Application.Queries.GetLearner;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;
using SFA.DAS.LearnerData.Application.Queries.GetSearch;
using SFA.DAS.LearnerData.Services;

namespace SFA.DAS.LearnerData.Api.Controllers;

[Route("providers/{ukprn:long}/learners")]
[ApiVersion("1.0")]
[ApiController]
public class LearnersController(ISender sender, IPagedLinkHeaderService pagedLinkHeaderService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    public async Task<IActionResult> Search(long ukprn, [FromQuery] int academicYear, [FromQuery] int page, [FromQuery] int? pageSize, string sortColumn, bool sortDescending, string filter)
    {
        var query = new GetSearchQuery(ukprn, academicYear, page, pageSize, sortColumn, sortDescending, filter);

        var result = await sender.Send(query);

        var pageLinks = pagedLinkHeaderService.GetPageLinks(query, result);

        Response?.Headers.Add(pageLinks);

        var response = new GetSearchResponse
        {
            LastSubmissionDate = result.LastSubmissionDate,
            Data = result.Items.Select(item => new GetSearchResponseItem
            {
                Id = item.Id,
                CreatedDate = item.CreatedDate,
                UpdatedDate = item.UpdatedDate,
                Uln = item.Uln,
                Ukprn = item.Ukprn,
                FirstName = item.FirstName,
                LastName = item.LastName,
                Email = item.Email,
                Dob = item.Dob,
                AcademicYear = item.AcademicYear,
                StartDate = item.StartDate,
                PlannedEndDate = item.PlannedEndDate,
                PercentageLearningToBeDelivered = item.PercentageLearningToBeDelivered,
                EpaoPrice = item.EpaoPrice,
                TrainingPrice = item.TrainingPrice,
                AgreementId = item.AgreementId,
                ConsumerReference = item.ConsumerReference,
                CorrelationId = item.CorrelationId,
                ReceivedDate = item.ReceivedDate,
                StandardCode = item.StandardCode,
                IsFlexiJob = item.IsFlexiJob,
                PlannedOTJTrainingHours = item.PlannedOTJTrainingHours
            }),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };

        return Ok(response);
    }

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    [Route("{uln:long}/academicyears/{academicYear:int}/standardcodes/{standardCode}")]
    public async Task<IActionResult> Save([FromBody] SaveLearnerRequest request)
    {
        var command = new SaveLearnerCommand
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
            IsFlexiJob = request.IsFlexiJob,
            PlannedOTJTrainingHours = request.PlannedOTJTrainingHours
        };

        var learnerId = await sender.Send(command);
        var location = $"providers/{request.Ukprn}/learners/{learnerId}";

        Response?.Headers.Add(new KeyValuePair<string, StringValues>("location", location));

        return Created();
    }


    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Route("{uln:long}/academicyears/{academicYear:int}/standardcodes/{standardCode:int}")]
    public async Task<IActionResult> GetSingle(long ukprn, long uln, int academicYear, int standardCode)
    {
        var query = new GetLearnerQuery(ukprn, uln, standardCode, academicYear);

        var result = await sender.Send(query);

        if (!result.Found)
        {
            return NotFound();
        }

        return Ok(new GetLearnerResponse
        {
            Id = result.Id,
            CreatedDate = result.CreatedDate,
            UpdatedDate = result.UpdatedDate,
            Uln = result.Uln,
            Ukprn = result.Ukprn,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email,
            Dob = result.Dob,
            AcademicYear = result.AcademicYear,
            StartDate = result.StartDate,
            PlannedEndDate = result.PlannedEndDate,
            PercentageLearningToBeDelivered = result.PercentageLearningToBeDelivered,
            EpaoPrice = result.EpaoPrice,
            TrainingPrice = result.TrainingPrice,
            AgreementId = result.AgreementId,
            ConsumerReference = result.ConsumerReference,
            CorrelationId = result.CorrelationId,
            ReceivedDate = result.ReceivedDate,
            StandardCode = result.StandardCode,
            IsFlexiJob = result.IsFlexiJob,
            PlannedOTJTrainingHours = result.PlannedOTJTrainingHours
        });
    }


    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Route("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var command = new GetLearnerByIdQuery(id);

        var result = await sender.Send(command);

        if (!result.Found)
        {
            return new NotFoundResult();
        }

        return Ok(new GetLearnerByIdResponse
        {
            Id = result.Id,
            CreatedDate = result.CreatedDate,
            UpdatedDate = result.UpdatedDate,
            Uln = result.Uln,
            Ukprn = result.Ukprn,
            FirstName = result.FirstName,
            LastName = result.LastName,
            Email = result.Email,
            Dob = result.Dob,
            AcademicYear = result.AcademicYear,
            StartDate = result.StartDate,
            PlannedEndDate = result.PlannedEndDate,
            PercentageLearningToBeDelivered = result.PercentageLearningToBeDelivered,
            EpaoPrice = result.EpaoPrice,
            TrainingPrice = result.TrainingPrice,
            AgreementId = result.AgreementId,
            ConsumerReference = result.ConsumerReference,
            CorrelationId = result.CorrelationId,
            ReceivedDate = result.ReceivedDate,
            StandardCode = result.StandardCode,
            IsFlexiJob = result.IsFlexiJob,
            PlannedOTJTrainingHours = result.PlannedOTJTrainingHours
        });
    }
}