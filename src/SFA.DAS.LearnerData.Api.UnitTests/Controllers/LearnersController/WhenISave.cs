using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenISave
{
    [Test, MoqAutoData]
    public async Task Then_BadRequest_Response_Is_Returned_When_Ukprn_Does_Not_Match(
        SaveLearnerRequest request,
        long ukprn,
        long uln,
        int academicYear,
        int id,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        request.Uln = uln;
        request.AcademicYear = academicYear;

        var result = await sut.Save(ukprn, uln, academicYear, request);

        var actionResult = result as BadRequestResult;
        actionResult.Should().NotBeNull();
    }

    [Test, MoqAutoData]
    public async Task Then_BadRequest_Response_Is_Returned_When_Uln_Does_Not_Match(
        SaveLearnerRequest request,
        long ukprn,
        long uln,
        int academicYear,
        int id,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        request.Ukprn = ukprn;
        request.AcademicYear = academicYear;

        var result = await sut.Save(ukprn, uln, academicYear, request);

        var actionResult = result as BadRequestResult;
        actionResult.Should().NotBeNull();
    }

    [Test, MoqAutoData]
    public async Task Then_BadRequest_Response_Is_Returned_When_AcademicYear_Does_Not_Match(
        SaveLearnerRequest request,
        long ukprn,
        long uln,
        int academicYear,
        int id,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        request.Ukprn = ukprn;
        request.Uln = uln;

        var result = await sut.Save(ukprn, uln, academicYear, request);

        var actionResult = result as BadRequestResult;
        actionResult.Should().NotBeNull();
    }

    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learner_Exists(
        SaveLearnerRequest request,
        long ukprn,
        long uln,
        int academicYear,
        int id,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        request.Ukprn = ukprn;
        request.Uln = uln;
        request.AcademicYear = academicYear;

        var response = new SaveLearnerCommandResponse { Id = id, Result = SaveLearnerResult.Updated };

        sender
            .Setup(x => x.Send(It.Is<SaveLearnerCommand>(ctx =>
                ctx.AcademicYear == request.AcademicYear
                && ctx.Uln == request.Uln
                && ctx.Ukprn == request.Ukprn
                && ctx.FirstName == request.FirstName
                && ctx.LastName == request.LastName
                && ctx.Email == request.Email
                && ctx.Dob == request.Dob
                && ctx.AcademicYear == request.AcademicYear
                && ctx.StartDate == request.StartDate
                && ctx.PlannedEndDate == request.PlannedEndDate
                && ctx.PercentageLearningToBeDelivered == request.PercentageLearningToBeDelivered
                && ctx.EpaoPrice == request.EpaoPrice
                && ctx.TrainingPrice == request.TrainingPrice
                && ctx.AgreementId == request.AgreementId
                && ctx.ConsumerReference == request.ConsumerReference
                && ctx.CorrelationId == request.CorrelationId
                && ctx.ReceivedDate == request.ReceivedDate
                && ctx.StandardCode == request.StandardCode
                && ctx.IsFlexiJob == request.IsFlexiJob
                && ctx.PlannedOTJTrainingHours == request.PlannedOTJTrainingHours), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Verifiable();

        var result = await sut.Save(ukprn, uln, academicYear, request);
        result.Should().NotBeNull();

        var actionResult = result as OkResult;
        actionResult.Should().NotBeNull();
        
        sender.Verify();
    }

   [Test, MoqAutoData]
    public async Task Then_Created_Response_Is_Returned_When_Learner_Does_Not_Exist(
        SaveLearnerRequest request,
        long ukprn,
        long uln,
        int academicYear,
        int id,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        request.Ukprn = ukprn;
        request.Uln = uln;
        request.AcademicYear = academicYear;

        var response = new SaveLearnerCommandResponse { Id = id, Result = SaveLearnerResult.Created };

        sender
            .Setup(x => x.Send(It.Is<SaveLearnerCommand>(ctx =>
                ctx.AcademicYear == request.AcademicYear
                && ctx.Uln == request.Uln
                && ctx.Ukprn == request.Ukprn
                && ctx.FirstName == request.FirstName
                && ctx.LastName == request.LastName
                && ctx.Email == request.Email
                && ctx.Dob == request.Dob
                && ctx.AcademicYear == request.AcademicYear
                && ctx.StartDate == request.StartDate
                && ctx.PlannedEndDate == request.PlannedEndDate
                && ctx.PercentageLearningToBeDelivered == request.PercentageLearningToBeDelivered
                && ctx.EpaoPrice == request.EpaoPrice
                && ctx.TrainingPrice == request.TrainingPrice
                && ctx.AgreementId == request.AgreementId
                && ctx.ConsumerReference == request.ConsumerReference
                && ctx.CorrelationId == request.CorrelationId
                && ctx.ReceivedDate == request.ReceivedDate
                && ctx.StandardCode == request.StandardCode
                && ctx.IsFlexiJob == request.IsFlexiJob
                && ctx.PlannedOTJTrainingHours == request.PlannedOTJTrainingHours), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response)
            .Verifiable();

        var result = await sut.Save(ukprn, uln, academicYear, request);
        result.Should().NotBeNull();

        var actionResult = result as CreatedAtActionResult;
        actionResult.Should().NotBeNull();
        
        sender.Verify();
    }
}