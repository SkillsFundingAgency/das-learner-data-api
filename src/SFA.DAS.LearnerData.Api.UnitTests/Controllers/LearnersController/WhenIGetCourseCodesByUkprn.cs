using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetCourseCodesByUkprn;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGetCourseCodesByUkprn
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Course_Codes_Returned(
        long ukprn,
        GetCourseCodesByUkprnResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut)
    {
        sender
            .Setup(x => x.Send(It.Is<GetCourseCodesByUkprnQuery>(ctx => ctx.Ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult)
            .Verifiable();

        var result = await sut.GetCourseCodesByUkprn(ukprn) as OkObjectResult;

        result.Should().NotBeNull();

        var response = result.Value as GetCourseCodesByUkprnResult;
        response.Should().BeEquivalentTo(queryResult);

        sender.Verify();
    }
}