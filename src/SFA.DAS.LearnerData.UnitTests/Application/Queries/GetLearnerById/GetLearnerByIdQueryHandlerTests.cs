using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetLearnerById;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetLearnerById;

public class GetLearnerByIdQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_GetById_When_Learner_Exists(
        GetLearnerByIdQuery query,
        Learner learner,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetLearnerByIdQueryHandler sut
    )
    {
        repository
            .Setup(x => x.GetById(query.Id, It.IsAny<CancellationToken>())).ReturnsAsync(learner)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(learner, options => options.ExcludingMissingMembers());
        result.Found.Should().BeTrue();
        
        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_GetById_When_Learner_Does_Not_Exist(
        GetLearnerByIdQuery query,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetLearnerByIdQueryHandler sut
    )
    {
        repository
            .Setup(x => x.GetById(query.Id, It.IsAny<CancellationToken>())).ReturnsAsync(() => null)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        
        repository.Verify();
    }
}