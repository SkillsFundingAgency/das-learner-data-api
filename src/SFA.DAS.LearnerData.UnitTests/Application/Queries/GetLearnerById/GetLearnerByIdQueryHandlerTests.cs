using AutoFixture;
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
        [Frozen] Mock<ILearnerRepository> repository,
        GetLearnerByIdQueryHandler sut
    )
    {
        learner.Ukprn = query.Ukprn; // Ensure the learner's UKPRN matches the query's UKPRN
        repository
            .Setup(x => x.GetById(query.Id, It.IsAny<CancellationToken>())).ReturnsAsync(learner)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(learner.Id);
        result.Uln.Should().Be(learner.Uln);
        result.Ukprn.Should().Be(learner.Ukprn);
        result.FirstName.Should().Be(learner.FirstName);
        result.LastName.Should().Be(learner.LastName);
        result.Email.Should().Be(learner.Email);
        result.Dob.Should().Be(learner.Dob);
        result.AcademicYear.Should().Be(learner.AcademicYear);
        result.StartDate.Should().Be(learner.StartDate);
        result.PercentageLearningToBeDelivered.Should().Be(learner.PercentageLearningToBeDelivered);
        result.EpaoPrice.Should().Be(learner.EpaoPrice);
        result.TrainingPrice.Should().Be(learner.TrainingPrice);
        result.AgreementId.Should().Be(learner.AgreementId);
        result.TrainingCode.Should().Be(learner.TrainingCode);
        result.TrainingName.Should().Be(learner.TrainingName);
        result.LearningType.Should().Be(learner.LearningType);
        result.IsFlexiJob.Should().Be(learner.IsFlexiJob);
        result.PlannedOTJTrainingHours.Should().Be(learner.PlannedOTJTrainingHours);
        result.ReceivedDate.Should().Be(learner.ReceivedDate);
        result.CorrelationId.Should().Be(learner.CorrelationId);
        result.ConsumerReference.Should().Be(learner.ConsumerReference);
        result.ApprenticeshipId.Should().Be(learner.ApprenticeshipId);
        result.Found.Should().BeTrue();
        
        repository.Verify();
    }

    [Test]
    public async Task Handle_GetById_And_Validate_Mapping_Of_LearningType_When_Null()
    {
        var fixture = new Fixture();

        var query = fixture.Create<GetLearnerByIdQuery>();
        var learner = fixture.Build<Learner>()
            .Without(x => x.LearningType)
            .With(x => x.Id, query.Id)
            .With(x => x.Ukprn, query.Ukprn)
            .Create();

        var repository = new Mock<ILearnerRepository>();
        GetLearnerByIdQueryHandler sut = new GetLearnerByIdQueryHandler(repository.Object);
 
        repository
            .Setup(x => x.GetById(query.Id, It.IsAny<CancellationToken>())).ReturnsAsync(learner)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.LearningType.Should().BeNull();
    }

    [Test, MoqAutoData]
    public async Task Handle_GetById_When_Learner_Exists_But_Is_Not_assigned_To_This_Provider(
        GetLearnerByIdQuery query,
        Learner learner,
        [Frozen] Mock<ILearnerRepository> repository,
        GetLearnerByIdQueryHandler sut
    )
    {
        learner.Ukprn = query.Ukprn + 1; // Ensure the learner's UKPRN matches the query's UKPRN
        repository
            .Setup(x => x.GetById(query.Id, It.IsAny<CancellationToken>())).ReturnsAsync(learner)
            .Verifiable();

        var result = await sut.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();

        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_GetById_When_Learner_Does_Not_Exist(
        GetLearnerByIdQuery query,
        [Frozen] Mock<ILearnerRepository> repository,
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