using System.Net;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Commands.CreateLearner;
using SFA.DAS.LearnerData.Application.Queries.GetAll;
using SFA.DAS.LearnerData.Application.Queries.GetLearner;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;

namespace SFA.DAS.LearnerData.Api.Controllers;

[Route("providers/{ukprn:long}/learners")]
[ApiVersion("1.0")]
[ApiController]
public class LearnersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetForProvider(long ukprn)
    {
        var query = new GetForProviderQuery(ukprn);

        var result = await sender.Send(query);

        if (!result.Found)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(new GetForProviderResponse
        {
            Learners = result.Learners.Select(learner => new GetForProviderResponseItem
            {
                Id = learner.Id,
                Uln = learner.Uln,
                Ukprn = learner.Ukprn,
                FirstName = learner.FirstName,
                LastName = learner.LastName,
                Email = learner.Email,
                Dob = learner.Dob,
                AcademicYear = learner.AcademicYear,
                StartDate = learner.StartDate,
                PlannedEndDate = learner.PlannedEndDate,
                PercentageLearningToBeDelivered = learner.PercentageLearningToBeDelivered,
                EpaoPrice = learner.EpaoPrice,
                TrainingPrice = learner.TrainingPrice,
                AgreementId = learner.AgreementId,
                ConsumerReference = learner.ConsumerReference,
                CorrelationId = learner.CorrelationId,
                ReceivedDate = learner.ReceivedDate,
                StandardCode = learner.StandardCode,
                IsFlexiJob = learner.IsFlexiJob,
                PlannedOTJTrainingHours = learner.PlannedOTJTrainingHours
            })
        });
    }

    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.Created)]
    public async Task<IActionResult> Create([FromBody] CreateLearnerRequest request)
    {
        var command = new CreateLearnerCommand
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

        await sender.Send(command);

        return new CreatedResult();
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [Route("{uln:long}/agreements/{agreementId}/academicyears/{academicYear:int}")]
    public async Task<IActionResult> GetSingle(long ukprn, long uln, string agreementId, int academicYear)
    {
        var query = new GetLearnerQuery(ukprn, uln, agreementId, academicYear);

        var result = await sender.Send(query);

        if (!result.Found)
        {
            return new NotFoundResult();
        }

        return new OkObjectResult(new GetLearnerResponse
        {
            Id = result.Id,
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

    [HttpPut]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("{uln:long}/agreements/{agreementId}/academicyears/{academicYear:int}")]
    public async Task<IActionResult> Put()
    {
        throw new NotImplementedException();
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

        return new OkObjectResult(new GetLearnerByIdResponse
        {
            Id = result.Id,
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
    [Route("academic-year")]
    public async Task<IActionResult> GetByAcademicYear([FromQuery] int academicYear, [FromQuery] int page = 1, [FromQuery] int pageSize = Int32.MaxValue)
    {
        throw new NotImplementedException();
    }
}