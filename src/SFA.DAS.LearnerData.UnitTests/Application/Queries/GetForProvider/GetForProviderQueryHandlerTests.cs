using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetAll;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetForProvider;

public class GetForProviderQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_GetForProvider_When_Learners_Exist(
        GetForProviderQuery query,
        GetForProviderResult queryResult,
        List<Learner> learners,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetForProviderQueryHandler sut
    )
    {
        repository
            .Setup(x => x.GetForProvider(query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(learners)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Learners.Should().BeEquivalentTo(learners, options => options.ExcludingMissingMembers());
        result.Found.Should().BeTrue();
        
        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_GetForProvider_When_Learners_Empty(
        GetForProviderQuery query,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetForProviderQueryHandler sut
    )
    {
        repository
            .Setup(x => x.GetForProvider(query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Learner>())
            .Verifiable();
        
        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        
        repository.Verify();
    }
    
    [Test, MoqAutoData]
    public async Task Handle_GetForProvider_When_Learners_Null(
        GetForProviderQuery query,
        [Frozen] Mock<ILearnerDataRepository> repository,
        GetForProviderQueryHandler sut
    )
    {
        repository
            .Setup(x => x.GetForProvider(query.Ukprn, It.IsAny<CancellationToken>())).ReturnsAsync(() => null)
            .Verifiable();
        
        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        
        repository.Verify();
    }
}