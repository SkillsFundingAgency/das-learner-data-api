using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries;
using SFA.DAS.LearnerData.Application.Queries.GetByAcademicYear;
using SFA.DAS.LearnerData.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGetByAcademicYear
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learners_Returned(
        GetByAcademicYearQuery query,
        GetByAcademicYearResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<IPagedLinkHeaderService> pagedLinkService,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        sender
            .Setup(x => x.Send(It.Is<GetByAcademicYearQuery>(ctx => ctx.UkPrn == query.UkPrn
                                                                    && ctx.AcademicYear == query.AcademicYear
                                                                    && ctx.Page == query.Page
                                                                    && ctx.PageSize == query.PageSize
                                                                    && ctx.Limit == query.Limit
                                                                    && ctx.Offset == query.Offset), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();

        pagedLinkService
            .Setup(x => x.GetPageLinks(It.IsAny<PagedQuery>(), It.IsAny<PagedQueryResult<GetByAcademicYearResult>>())).Returns(new KeyValuePair<string, StringValues>())
            .Verifiable();

        var result = await sut.GetByAcademicYear(query.UkPrn, query.AcademicYear, query.Page, query.PageSize);
        result.Should().NotBeNull();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var response = okResult.Value as GetByAcademicYearResult;
        response.Should().BeEquivalentTo(queryResult, options => options.ExcludingMissingMembers());

        sender.Verify();
    }
}