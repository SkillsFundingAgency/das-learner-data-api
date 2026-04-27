using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Repositories;

public class WhenIGetCourseListForUkprn
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
        _dbContext.Dispose();
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_Empty_Result_When_No_CourseCodes_Exist(
        long ukprn, bool excludeApproved, string maxStartDate, string excludeUlns)
    {
        // Act
        var result = await _repository.GetCourseList(ukprn, excludeApproved, maxStartDate, excludeUlns, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [TestCase(100, true, null, null, 3)]
    [TestCase(100, false, null, null, 4)]
    [TestCase(100, false, null, null, 4)]
    [TestCase(100, false, "2026-08-01", null, 4)]
    [TestCase(100, false, null, "123456789,123456788", 3)]
    public async Task Then_Returns_Courses_For_Query(
        long ukprn, bool excludeApproved, string? maxStartDate, string? excludeUlns, int expectedCount)
    {

        var learners = new List<Learner>()
        {
            new() { Ukprn = ukprn , TrainingCode = "1", TrainingName = "Course1", StartDate = new DateTime(2027,01,01), Uln = 123456789 },
            new() { Ukprn = ukprn , TrainingCode = "1", TrainingName = "Course1", StartDate = new DateTime(2026, 01,01), Uln = 123456788 },
            new() { Ukprn = ukprn , TrainingCode = "2", TrainingName = "Course2", StartDate = new DateTime(2026,01,01)},
            new() { Ukprn = ukprn , TrainingCode = "2A", TrainingName = "Course2A", StartDate = new DateTime(2026,04,01), ApprenticeshipId = 999},
            new() { Ukprn = ukprn , TrainingCode = "3", TrainingName = "Course3", StartDate = new DateTime(2026, 01, 01)},
            new() { Ukprn = ukprn+1 , TrainingCode = "3", TrainingName = "Course3", StartDate = new DateTime(2027,01,01)},
        };

        learners.ForEach(t => t.Ukprn = ukprn);
        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCourseList(ukprn, excludeApproved, maxStartDate, excludeUlns, _cancellationToken);

        // Assert
        result.Count.Should().Be(expectedCount);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_CourseCodes_And_Names_If_Present(
    long ukprn)
    {

        var learners = new List<Learner>()
        {
            new() { Ukprn = ukprn , TrainingCode = "1", TrainingName = null, StartDate = new DateTime(2027,01,01), Uln = 123456789 },
            new() { Ukprn = ukprn , TrainingCode = "1", TrainingName = "Course1", StartDate = new DateTime(2026, 01,01), Uln = 123456788 },
            new() { Ukprn = ukprn , TrainingCode = "2", TrainingName = "Course2", StartDate = new DateTime(2026,01,01)},
            new() { Ukprn = ukprn , TrainingCode = "2A", TrainingName = "Course2A", StartDate = new DateTime(2026,04,01), ApprenticeshipId = 999},
            new() { Ukprn = ukprn , TrainingCode = "3", TrainingName = "Course3", StartDate = new DateTime(2026, 01, 01)},
            new() { Ukprn = ukprn , TrainingCode = "4", TrainingName = null, StartDate = new DateTime(2026, 01, 01)},
            new() { Ukprn = ukprn+1 , TrainingCode = "3", TrainingName = "Course3", StartDate = new DateTime(2027,01,01)},
        };

        learners.ForEach(t => t.Ukprn = ukprn);
        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCourseList(ukprn, false, null, null, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(5);
        result.First(x => x.TrainingCode == "1").TrainingName.Should().Be("Course1");
        result.First(x => x.TrainingCode == "4").TrainingName.Should().BeNull();
    }
}