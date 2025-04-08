using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries.GetAll;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGetForProvider
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learners_Returned(
        GetForProviderQuery query,
        GetForProviderResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetForProviderQuery>(ctx => ctx.Ukprn == query.Ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();

        var result = await sut.GetForProvider(query.Ukprn);
        result.Should().NotBeNull();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult.Value as GetForProviderResponse;
        response.Should().BeEquivalentTo(queryResult, options => options.ExcludingMissingMembers());
        
        sender.Verify();
    }
    
    [Test, MoqAutoData]
    public async Task Then_NotFound_Response_Is_Returned_When_Learners_Do_Not_Exist(
        GetForProviderQuery query,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetForProviderQuery>(ctx => ctx.Ukprn == query.Ukprn), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetForProviderResult())
            .Verifiable();

        var result = await sut.GetForProvider(query.Ukprn);
        result.Should().NotBeNull();

        var okResult = result as NotFoundResult;
        okResult.Should().NotBeNull();
        
        sender.Verify();
    }
}