using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetByAcademicYear;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetByAcademicYear;

public class GetByAcademicYearQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_GetForProvider_When_Learners_Exist(
        GetByAcademicYearQuery query,
        PagedResult<Learner> learners,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetByAcademicYearQueryHandler sut
    )
    {
        query.Page = 1;
        query.PageSize = 10;

        learners.Page = query.Page;
        learners.PageSize = query.PageSize.Value;
        
        repository
            .Setup(x => x.GetByAcademicYear(query.UkPrn, query.AcademicYear, query.Page, query.PageSize, query.Limit, query.Offset, It.IsAny<CancellationToken>())).ReturnsAsync(learners)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Items.Should().BeEquivalentTo(learners.Data, options => options.ExcludingMissingMembers());

        result.Page.Should().Be(query.Page);
        result.PageSize.Should().Be(query.PageSize);
        result.TotalItems.Should().Be(learners.TotalItems);

        repository.Verify();
    }
}