using System.Linq;
using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;
using SFA.DAS.LearnerData.Application.Queries.GetLearnersById;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGetLearnersById
{
    [Test, MoqAutoData]
    public async Task Then_BadRequest_Is_Returned_When_LearnerIds_Empty(
        long ukprn,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        var request = new GetLearnersByIdRequest { LearnerIds = [] };

        var result = await sut.GetLearnersById(ukprn, request);

        result.Should().BeOfType<BadRequestResult>();
        sender.Verify(x => x.Send(It.IsAny<GetLearnersByIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_BadRequest_Is_Returned_When_Request_Is_Null(
        long ukprn,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        var result = await sut.GetLearnersById(ukprn, null!);

        result.Should().BeOfType<BadRequestResult>();
        sender.Verify(x => x.Send(It.IsAny<GetLearnersByIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_BadRequest_Is_Returned_When_LearnerIds_Exceed_100(
        long ukprn,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        var request = new GetLearnersByIdRequest
        {
            LearnerIds = Enumerable.Range(1, 101).Select(i => (long)i).ToList()
        };

        var result = await sut.GetLearnersById(ukprn, request);

        result.Should().BeOfType<BadRequestResult>();
        sender.Verify(x => x.Send(It.IsAny<GetLearnersByIdQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_BadRequest_With_MissingIds_Is_Returned_When_Partial_Miss(
        long ukprn,
        GetLearnerByIdResult foundLearner,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        var missingId = foundLearner.Id + 1;
        var request = new GetLearnersByIdRequest
        {
            LearnerIds = [foundLearner.Id, missingId]
        };

        sender
            .Setup(x => x.Send(It.IsAny<GetLearnersByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([foundLearner])
            .Verifiable();

        var result = await sut.GetLearnersById(ukprn, request);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().BeEquivalentTo(new { MissingIds = new List<long> { missingId } });
        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_Duplicate_LearnerIds_Are_Deduped_Before_Query_And_Count_Check(
        long ukprn,
        GetLearnerByIdResult learner,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        var request = new GetLearnersByIdRequest
        {
            LearnerIds = [learner.Id, learner.Id, learner.Id]
        };

        sender
            .Setup(x => x.Send(
                It.Is<GetLearnersByIdQuery>(q =>
                    q.ukprn == ukprn && q.ids.SequenceEqual(new[] { learner.Id })),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([learner])
            .Verifiable();

        var result = await sut.GetLearnersById(ukprn, request);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<List<GetLearnerByIdResponse>>().Subject;
        response.Should().ContainSingle().Which.Id.Should().Be(learner.Id);
        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_All_Learners_Found(
        long ukprn,
        List<GetLearnerByIdResult> learners,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        var request = new GetLearnersByIdRequest
        {
            LearnerIds = learners.Select(x => x.Id).ToList()
        };

        sender
            .Setup(x => x.Send(
                It.Is<GetLearnersByIdQuery>(q => q.ukprn == ukprn && q.ids.SequenceEqual(request.LearnerIds.Distinct())),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(learners)
            .Verifiable();

        var result = await sut.GetLearnersById(ukprn, request);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeAssignableTo<List<GetLearnerByIdResponse>>().Subject;
        response.Should().HaveCount(learners.Count);
        response.Select(x => x.Id).Should().BeEquivalentTo(learners.Select(x => x.Id));
        sender.Verify();
    }
}
