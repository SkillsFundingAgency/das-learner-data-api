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

public class WhenIGetCourseCodesByUkprn
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
        long ukprn)
    {
        // Act
        var result = await _repository.GetCourseCodesByUkprn(ukprn, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0);
    }

    [Test, MoqAutoData]
    public async Task Then_Returns_CourseCodes_Give_Ukprn(
        long ukprn)
    {
        var learners = new List<Learner>()
        {
            new() { Ukprn = ukprn , TrainingCode = "1"},
            new() { Ukprn = ukprn , TrainingCode = "1"},
            new() { Ukprn = ukprn , TrainingCode = "2"},
            new() { Ukprn = ukprn , TrainingCode = "3"},
        };

        learners.ForEach(t => t.Ukprn = ukprn);
        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetCourseCodesByUkprn(ukprn, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(learners.Where(t => t.Ukprn == ukprn).Select(t => t.TrainingCode).Distinct());
    }
}