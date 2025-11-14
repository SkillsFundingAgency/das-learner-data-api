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
        GetLearnerByIdResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetLearnerByIdQuery>(ctx => ctx.Id == id && ctx.ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(queryResult)
            .Verifiable();

        var result = await sut.GetById(ukprn, id);
        result.Should().NotBeNull();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult.Value as GetLearnerByIdResponse;
        response.Should().BeEquivalentTo(queryResult, options => options.ExcludingMissingMembers());
        
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
            .Setup(x => x.Send(It.Is<GetLearnerByIdQuery>(ctx => ctx.Id == id && ctx.ukprn == ukprn), It.IsAny<CancellationToken>())).ReturnsAsync(new GetLearnerByIdResult())
            .Verifiable();

        var result = await sut.GetById(ukprn, id);
        result.Should().NotBeNull();

        var okResult = result as NotFoundResult;
        okResult.Should().NotBeNull();
        
        sender.Verify();
    }
}