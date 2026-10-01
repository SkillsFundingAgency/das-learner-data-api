using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetLearnersById;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetLearnersById;

public class GetLearnersByIdQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_Returns_Mapped_Learners_From_Repository(
        long ukprn,
        List<long> ids,
        List<Learner> learners,
        [Frozen] Mock<ILearnerRepository> repository,
        GetLearnersByIdQueryHandler sut
    )
    {
        repository
            .Setup(x => x.GetByIds(ukprn, ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync(learners)
            .Verifiable();

        var result = await sut.Handle(new GetLearnersByIdQuery(ukprn, ids), CancellationToken.None);

        result.Should().HaveCount(learners.Count);
        result.Select(x => x.Id).Should().BeEquivalentTo(learners.Select(x => x.Id));
        result.Select(x => x.Uln).Should().BeEquivalentTo(learners.Select(x => x.Uln));
        result.Select(x => x.Ukprn).Should().BeEquivalentTo(learners.Select(x => x.Ukprn));
        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Returns_Empty_List_When_Repository_Returns_None(
        long ukprn,
        List<long> ids,
        [Frozen] Mock<ILearnerRepository> repository,
        GetLearnersByIdQueryHandler sut
    )
    {
        repository
            .Setup(x => x.GetByIds(ukprn, ids, It.IsAny<CancellationToken>()))
            .ReturnsAsync([])
            .Verifiable();

        var result = await sut.Handle(new GetLearnersByIdQuery(ukprn, ids), CancellationToken.None);

        result.Should().BeEmpty();
        repository.Verify();
    }
}
