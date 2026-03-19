using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Messages;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Repositories;

[TestFixture]
public class WhenIAssignApprenticeshipId
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

        _repository = new LearnerRepository(_dbContext, Mock.Of<ILogger<LearnerRepository>>());

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
        Learner learner)
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
        AssignApprenticeshipIdCommand command)
    {
        var ex = () => _repository.AssignApprenticeshipId(command, _cancellationToken);

        await ex.Should().ThrowAsync<KeyNotFoundException>().WithMessage($"Learner with ID {command.LearnerDataId} not found.");
    }

    [Test, MoqAutoData]
    public async Task AssignApprenticeshipId_Throws_Exception_When_Learner_Does_Not_Belong_To_Provider(
        AssignApprenticeshipIdCommand command,
        Learner learner)
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
        Learner learner)
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
        Learner learner)
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

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Exclude_Learners_With_ApprenticeshipId_Assigned(
        AssignApprenticeshipIdCommand command,
        long providerId,
        List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
        });
        learners[0].ApprenticeshipId = null;
        learners[0].StartDate = new DateTime(2025, 05, 01);

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", true, null, 2025, null, "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(1);
        results.First().Should().BeEquivalentTo(learners[0]);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Include_Learners_With_ApprenticeshipId_Assigned(
        AssignApprenticeshipIdCommand command,
        long providerId,
        List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, null, "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(3);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Include_Learners_When_StartYear_Matches(
        AssignApprenticeshipIdCommand command,
        long providerId,
        List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, null, "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(3);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Not_Include_Learners_When_StartYear_Not_Matches(
        AssignApprenticeshipIdCommand command,
        long providerId,
        List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2024, null, "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Include_Learners_When_StartYear_And_StartMonth_Matches(
        AssignApprenticeshipIdCommand command,
        long providerId,
        List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
        });
        learners[0].StartDate = new DateTime(2025, 01, 01);

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, 01, 2025, null, "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(1);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Include_Learners_When_StartDate_Is_Less_Than_MaxStartDate(
       long providerId,
       List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
        });
        learners[0].StartDate = new DateTime(2025, 11, 02);

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, "2025-12-01", "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(learners.Count);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Exclude_Learners_When_StartDate_Is_Grater_Than_MaxStartDate(
       long providerId,
       List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
        });
        learners[0].StartDate = new DateTime(2025, 12, 06);

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, "2025-12-01", "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(learners.Count - 1);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Remove_Exluded_Ulns(
        AssignApprenticeshipIdCommand command,
        long providerId,
        List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
            x.ApprenticeshipId = null;
        });
        learners[0].Uln = 12345;

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", true, null, 2025, null, "12345", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(learners.Count - 1);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Return_All_Records_When_Excluded_Ulns_Is_Empty(
        AssignApprenticeshipIdCommand command,
        long providerId,
        List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
            x.ApprenticeshipId = null;
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", true, null, 2025, null, "", null, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(learners.Count);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Include_Learners_When_CourseCode_Matches(
       AssignApprenticeshipIdCommand command,
       long providerId,
       int courseCode,
       List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
            x.StandardCode = courseCode;
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, null, "", courseCode, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(learners.Count);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Not_Include_Learners_When_CourseCode_Not_Matches(
       AssignApprenticeshipIdCommand command,
       long providerId,
       int courseCode,
       List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, null, "", courseCode, null, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Include_Learners_When_LearningType_Matches(
       AssignApprenticeshipIdCommand command,
       long providerId,
       LearningType learningType,
       List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
            x.LearningType = learningType;
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, null, "", null, learningType, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(learners.Count);
    }

    [Test, MoqAutoData]
    public async Task When_Learner_Search_Should_Not_Include_Learners_When_LearningType_Not_Matches(
       AssignApprenticeshipIdCommand command,
       long providerId,
       List<Learner> learners)
    {
        learners.ForEach(x =>
        {
            x.Ukprn = providerId;
            x.StartDate = new DateTime(2025, 05, 01);
            x.LearningType = LearningType.Apprenticeship;
        });

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var response = await _repository.Search(providerId, 1, 10, 1000, 0, null, false, "", false, null, 2025, null, "", null, LearningType.ApprenticeshipUnit, _cancellationToken);
        var results = response.Data.ToList();
        results.Count.Should().Be(0);
    }
}