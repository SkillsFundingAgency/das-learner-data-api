using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetCourseCodesByUkprn;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetCourseCodesByUkprn;

public class GetCourseCodesByUkprnHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_GetCourseCodesByUkprn(
       GetCourseCodesByUkprnQuery query,
       List<int> courseCodes,
       [Frozen] Mock<ILearnerRepository> repository,
       GetCourseCodesByUkprnQueryHandler sut)
    {
        repository
            .Setup(x => x.GetCourseCodesByUkprn(It.Is<long>(t => t == query.Ukprn),
            default)).ReturnsAsync(courseCodes)
            .Verifiable();

        var result = await sut.Handle(query, default);
        result.Should().NotBeNull();
        result.CourseCodes.Should().BeEquivalentTo(courseCodes);
        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Returns_Empty_Result_When_Nocourses_Exists(
       GetCourseCodesByUkprnQuery query,
       [Frozen] Mock<ILearnerRepository> repository,
       GetCourseCodesByUkprnQueryHandler sut)
    {
        repository
            .Setup(x => x.GetCourseCodesByUkprn(It.Is<long>(t => t == query.Ukprn),
            default)).ReturnsAsync([])
            .Verifiable();

        var result = await sut.Handle(query, default);
        result.Should().NotBeNull();
        result.CourseCodes.Count.Should().Be(0);
        repository.Verify();
    }
}