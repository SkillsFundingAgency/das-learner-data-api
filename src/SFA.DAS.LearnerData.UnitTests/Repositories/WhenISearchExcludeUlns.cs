using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;

namespace SFA.DAS.LearnerData.UnitTests.Repositories;

[TestFixture]
public class WhenISearchExcludeUlns
{
    private LearnerRepository _repository;
    private LearnerDataDbContext _dbContext;
    private CancellationToken _cancellationToken;
    private const long Ukprn = 888001;

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
        _dbContext.Dispose();
    }

    [Test]
    public async Task Then_Excludes_Ulns_From_Result()
    {
        var ulnIncluded = 1000000001;
        var ulnExcludedA = 1000000002;
        var ulnExcludedB = 1000000003;

        _dbContext.Learners.AddRange(
            CreateLearner(Ukprn, ulnIncluded),
            CreateLearner(Ukprn, ulnExcludedA),
            CreateLearner(Ukprn, ulnExcludedB));
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Search(
            Ukprn, 1, 100, 100, 0,
            nameof(Learner.StartDate), true, "", false,
            null, 0, "",
            [ulnExcludedA, ulnExcludedB],
            null,null,
            _cancellationToken);

        result.Data.Should().ContainSingle(l => l.Uln == ulnIncluded);
        result.TotalItems.Should().Be(1);
    }

    private static Learner CreateLearner(long ukprn, long uln) =>
        new()
        {
            Ukprn = ukprn,
            Uln = uln,
            FirstName = "F",
            LastName = "L",
            ConsumerReference = "cr",
            Dob = new DateTime(1990, 1, 1),
            AcademicYear = 2024,
            StartDate = new DateTime(2024, 9, 1),
            PlannedEndDate = new DateTime(2026, 7, 1),
            EpaoPrice = 0,
            TrainingPrice = 0,
            TrainingCode = "1",
            ReceivedDate = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid(),
        };
}
