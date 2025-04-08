using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries.GetLearner;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGet
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learner_Exists(
        GetLearnerQuery query,
        GetLearnerResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetLearnerQuery>(ctx => ctx.Ukprn == query.Ukprn && ctx.Uln == query.Uln && ctx.AcademicYear == query.AcademicYear && ctx.StandardCode == query.StandardCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();

        var result = await sut.GetSingle(query.Ukprn, query.Uln, query.AcademicYear, query.StandardCode);
        result.Should().NotBeNull();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult.Value as GetLearnerResponse;
        response.Should().BeEquivalentTo(queryResult, options => options.ExcludingMissingMembers());

        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_NotFound_Response_Is_Returned_When_Learner_Does_Not_Exist(
        GetLearnerQuery query,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetLearnerQuery>(ctx => ctx.Ukprn == query.Ukprn && ctx.Uln == query.Uln && ctx.AcademicYear == query.AcademicYear && ctx.StandardCode == query.StandardCode), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetLearnerResult())
            .Verifiable();

        var result = await sut.GetSingle(query.Ukprn, query.Uln, query.AcademicYear, query.StandardCode);
        result.Should().NotBeNull();

        var okResult = result as NotFoundResult;
        okResult.Should().NotBeNull();

        sender.Verify();
    }
}