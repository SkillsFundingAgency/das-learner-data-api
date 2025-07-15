using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetSearch;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetSearch;

public class GetSearchQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_GetForProvider_When_Learners_Exist(
        GetSearchQuery query,
        PagedResult<Learner> learners,
        DateTime? lastSubmissionDate,
        [Frozen] Mock<ILearnerRepository> repository,
        GetSearchQueryHandler sut
    )
    {
        query.Page = 1;
        query.PageSize = 10;

        learners.Page = query.Page;
        learners.PageSize = query.PageSize.Value;

        repository
            .Setup(x => x.Search(query.UkPrn, query.AcademicYear, query.Page, query.PageSize, query.Limit, query.Offset, query.SortColumn, query.SortDescending, query.Filter, query.ExcludeApproved, It.IsAny<CancellationToken>())).ReturnsAsync(learners)
            .Verifiable();

        repository
            .Setup(x => x.GetLastSubmissionDate(query.UkPrn, query.AcademicYear, It.IsAny<CancellationToken>())).ReturnsAsync(lastSubmissionDate)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(learners.Data, options => options.ExcludingMissingMembers());

        result.Page.Should().Be(query.Page);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalItems.Should().Be(learners.TotalItems);
        result.LastSubmissionDate.Should().Be(lastSubmissionDate);

        repository.Verify();
    }
}