using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Data;
using AutoFixture.NUnit3;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Repositories;

[TestFixture]
public class LearnerRepositoryTests
{
    private LearnerRepository _repository;
    private LearnerDataDbContext _dbContext;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void Setup()
    {
        _dbContext = new LearnerDataDbContext(new DbContextOptionsBuilder<LearnerDataDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
            .EnableSensitiveDataLogging()
            .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
            .Options);

        _repository = new LearnerRepository(_dbContext);

        _cancellationToken = CancellationToken.None;
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test, MoqAutoData]
    public async Task AssignApprenticeshipId_Updates_ApprenticeshipId_When_Learner_Matches_And_ApprenticeshipId_Is_Null(
        AssignApprenticeshipIdCommand command,
        Learner learner,
        [Frozen] Mock<ILearnerRepository> repository)
    {
        learner.Id = command.LearnerDataId;
        learner.Ukprn = command.Ukprn;
        learner.ApprenticeshipId = null;
        _dbContext.Learners.Add(learner);

        await _repository.AssignApprenticeshipId(command, _cancellationToken);

        var updatedLearner = await _dbContext.Learners
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == command.LearnerDataId, _cancellationToken);
        updatedLearner.Should().NotBeNull();
        updatedLearner.ApprenticeshipId.Should().Be(command.ApprenticeshipId);
    }

    [Test, MoqAutoData]
    public async Task AssignApprenticeshipId_Throws_Exception_When_Learner_Not_Found(
        AssignApprenticeshipIdCommand command,
        [Frozen] Mock<ILearnerRepository> repository)
    {
        var ex = () => _repository.AssignApprenticeshipId(command, _cancellationToken);

        await ex.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Learner with ID {command.LearnerDataId} not found.");
    }

    [Test, MoqAutoData]
    public async Task AssignApprenticeshipId_Throws_Exception_When_Learner_Does_Not_Belong_To_Provider(
        AssignApprenticeshipIdCommand command,
        Learner learner,
        [Frozen] Mock<ILearnerRepository> repository)
    {
        learner.Id = command.LearnerDataId;
        learner.Ukprn = command.Ukprn + 1; 
        _dbContext.Learners.Add(learner);

        var ex = () => _repository.AssignApprenticeshipId(command, _cancellationToken);
        await ex.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Learner with ID {command.LearnerDataId} not found for UKPRN {command.Ukprn}");
    }

    [Test, MoqAutoData]
    public async Task AssignApprenticeshipId_Throws_Exception_When_Learner_Already_Has_ApprenticeshipId_Assigned(
        AssignApprenticeshipIdCommand command,
        Learner learner,
        [Frozen] Mock<ILearnerRepository> repository)
    {
        learner.Id = command.LearnerDataId;
        learner.Ukprn = command.Ukprn;
        learner.ApprenticeshipId = 1234; 
        _dbContext.Learners.Add(learner);

        var ex = () => _repository.AssignApprenticeshipId(command, _cancellationToken);
        await ex.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Learner with ID {command.LearnerDataId} already has a different ApprenticeshipId assigned.");
    }

    [Test, MoqAutoData]
    public async Task AssignApprenticeshipId_Updates_ApprenticeshipId_When_Learner_Matches_And_ApprenticeshipId_Matches_the_incoming_Id(
        AssignApprenticeshipIdCommand command,
        Learner learner,
        [Frozen] Mock<ILearnerRepository> repository)
    {
        learner.Id = command.LearnerDataId;
        learner.Ukprn = command.Ukprn;
        learner.ApprenticeshipId = command.ApprenticeshipId;
        _dbContext.Learners.Add(learner);

        await _repository.AssignApprenticeshipId(command, _cancellationToken);

        var updatedLearner = await _dbContext.Learners
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == command.LearnerDataId, _cancellationToken);
        updatedLearner.Should().NotBeNull();
        updatedLearner.ApprenticeshipId.Should().Be(command.ApprenticeshipId);
    }

}