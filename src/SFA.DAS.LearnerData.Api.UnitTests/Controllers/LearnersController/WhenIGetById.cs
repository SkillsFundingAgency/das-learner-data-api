using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGetById
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learner_Exists(
        long id,
        long ukprn,
        GetLearnerByIdResult learner,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetLearnerByIdQuery>(ctx => ctx.Id == id && ctx.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(learner)
            .Verifiable();

        var result = await sut.GetById(ukprn, id);
        result.Should().NotBeNull();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult.Value as GetLearnerByIdResponse;
        response.Should().NotBeNull();
        response.Id.Should().Be(learner.Id);
        response.Uln.Should().Be(learner.Uln);
        response.Ukprn.Should().Be(learner.Ukprn);
        response.FirstName.Should().Be(learner.FirstName);
        response.LastName.Should().Be(learner.LastName);
        response.Email.Should().Be(learner.Email);
        response.Dob.Should().Be(learner.Dob);
        response.AcademicYear.Should().Be(learner.AcademicYear);
        response.StartDate.Should().Be(learner.StartDate);
        response.PercentageLearningToBeDelivered.Should().Be(learner.PercentageLearningToBeDelivered);
        response.EpaoPrice.Should().Be(learner.EpaoPrice);
        response.TrainingPrice.Should().Be(learner.TrainingPrice);
        response.AgreementId.Should().Be(learner.AgreementId);
        response.TrainingCode.Should().Be(learner.TrainingCode);
        response.TrainingName.Should().Be(learner.TrainingName);
        response.LearningType.Should().Be(learner.LearningType);
        response.IsFlexiJob.Should().Be(learner.IsFlexiJob);
        response.PlannedOTJTrainingHours.Should().Be(learner.PlannedOTJTrainingHours);
        response.ReceivedDate.Should().Be(learner.ReceivedDate);
        response.CorrelationId.Should().Be(learner.CorrelationId);
        response.ConsumerReference.Should().Be(learner.ConsumerReference);
        response.ApprenticeshipId.Should().Be(learner.ApprenticeshipId);

        sender.Verify();
    }
    
    [Test, MoqAutoData]
    public async Task Then_NotFound_Response_Is_Returned_When_Learner_Does_Not_Exist(
        int id,
        long ukprn,
        GetLearnerByIdResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetLearnerByIdQuery>(ctx => ctx.Id == id && ctx.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new GetLearnerByIdResult())
            .Verifiable();

        var result = await sut.GetById(ukprn, id);
        result.Should().NotBeNull();

        var okResult = result as NotFoundResult;
        okResult.Should().NotBeNull();
        
        sender.Verify();
    }
}