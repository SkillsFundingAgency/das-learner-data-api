using System.Net;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.LearnerData.Application.Commands;

namespace SFA.DAS.LearnerData.Api.Controllers;

[Route("providers/{ukprn:long}/learners")]
[ApiVersion("1.0")]
[ApiController]
public class LearnersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAll()
    {
        throw new NotImplementedException();
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
    [Route("{uln:long}/agreements/{agreementId}/academicyears/{academicYear:int}")]
    public async Task<IActionResult> GetSingle(long uln, string agreementId, int academicYear)
    {
        throw new NotImplementedException();
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
    [Route("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [Route("academic-year")]
    public async Task<IActionResult> GetByAcademicYear([FromQuery] int academicYear, [FromQuery] int page = 1, [FromQuery] int pageSize = Int32.MaxValue)
    {
        throw new NotImplementedException();
    }
}