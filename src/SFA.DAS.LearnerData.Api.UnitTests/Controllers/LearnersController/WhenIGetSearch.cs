using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries;
using SFA.DAS.LearnerData.Application.Queries.GetSearch;
using SFA.DAS.LearnerData.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGetSearch
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learners_Returned(
        long ukrprn,
        SearchLearnersRequest request,
        GetSearchResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<IPagedLinkHeaderService> pagedLinkService,
        [Greedy] Api.Controllers.ProviderLearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetSearchQuery>(ctx => ctx.UkPrn == ukrprn
                                                                    && ctx.StartMonth == request.StartMonth
                                                                    && ctx.StartYear == request.StartYear
                                                                    && ctx.ExcludeApproved == request.ExcludeApproved
                                                                    && ctx.Page == request.Page
                                                                    && ctx.PageSize == request.PageSize
                                                                    ), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();

        pagedLinkService
            .Setup(x => x.GetPageLinks(It.IsAny<PagedQuery>(), It.IsAny<PagedQueryResult<GetSearchResult>>())).Returns(new KeyValuePair<string, StringValues>())
            .Verifiable();

        var result = await sut.Search(ukrprn,request);
        result.Should().NotBeNull();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult.Value as GetSearchResponse;
        response.Should().BeEquivalentTo(queryResult, options => options.ExcludingMissingMembers());

        sender.Verify();
    }
}