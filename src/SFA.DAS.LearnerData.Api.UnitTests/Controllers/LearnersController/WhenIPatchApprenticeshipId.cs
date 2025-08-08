using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIPatchApprenticeshipId
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learner_is_Patched_Correctly(
        long id,
        long ukprn,
        PatchLearnerDataApprenticeshipIdRequest request,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<AssignApprenticeshipIdCommand>(ctx => ctx.LearnerDataId == id && ctx.Ukprn == ukprn && ctx.ApprenticeshipId == request.ApprenticeshipId),
                It.IsAny<CancellationToken>()))
            .Verifiable();

        var result = await sut.PatchApprenticeshipId(ukprn, id, request);
        result.Should().NotBeNull();

        result.Should().BeOfType<OkResult>();
        
        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_NotFound_Response_Is_Returned_When_Learner_is_Not_Matched_Correctly(
        long id,
        long ukprn,
        PatchLearnerDataApprenticeshipIdRequest request,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<AssignApprenticeshipIdCommand>(ctx => ctx.LearnerDataId == id && ctx.Ukprn == ukprn && ctx.ApprenticeshipId == request.ApprenticeshipId),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException("Record Not found"))
            .Verifiable();

        var result = await sut.PatchApprenticeshipId(ukprn, id, request);
        result.Should().NotBeNull();
        result.Should().BeOfType<NotFoundResult>();

        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_InternalServerIssue_Response_Is_Returned_When_Learner_update_errors(
        long id,
        long ukprn,
        PatchLearnerDataApprenticeshipIdRequest request,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<AssignApprenticeshipIdCommand>(ctx => ctx.LearnerDataId == id && ctx.Ukprn == ukprn && ctx.ApprenticeshipId == request.ApprenticeshipId),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApplicationException("Update error"))
            .Verifiable();

        var result = await sut.PatchApprenticeshipId(ukprn, id, request);
        result.Should().NotBeNull();
        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        sender.Verify();
    }
}