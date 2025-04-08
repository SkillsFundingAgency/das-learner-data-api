using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetLearner;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetLearner;

public class GetLearnerQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_Get_When_Learner_Exists(
        GetLearnerQuery query,
        Learner learner,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetLearnerQueryHandler sut
    )
    {
        repository
            .Setup(x => x.Get(query.Ukprn, query.Uln, query.AgreementId, query.AcademicYear, It.IsAny<CancellationToken>())).ReturnsAsync(learner)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(learner, options => options.ExcludingMissingMembers());
        result.Found.Should().BeTrue();
        
        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Get_When_Learner_Does_Not_Exist(
        GetLearnerQuery query,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetLearnerQueryHandler sut
    )
    {
        repository
            .Setup(x => x.Get(query.Ukprn, query.Uln, query.AgreementId, query.AcademicYear, It.IsAny<CancellationToken>())).ReturnsAsync(() => null)
            .Verifiable();
        
        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        
        repository.Verify();
    }
}