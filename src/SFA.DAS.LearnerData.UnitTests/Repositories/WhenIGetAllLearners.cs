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
public class WhenIGetAllLearners
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

    [Test]
    public async Task Then_Returns_All_Learners_When_ExcludeApproved_Is_False()
    {
        // Arrange
        var learners = new List<Learner>
        {
            new() { Id = 1, Ukprn = 12345, Uln = 1234567890, ApprenticeshipId = null },
            new() { Id = 2, Ukprn = 12345, Uln = 1234567891, ApprenticeshipId = 12345 },
            new() { Id = 3, Ukprn = 12345, Uln = 1234567892, ApprenticeshipId = null }
        };

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var page = 1;
        var pageSize = 100;
        var limit = 100;
        var offset = 0;
        bool excludeApproved = false;

        // Act
        var result = await _repository.GetAllLearners(page, pageSize, limit, offset, excludeApproved, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
    }

    [Test]
    public async Task Then_Returns_Only_Unapproved_Learners_When_ExcludeApproved_Is_True()
    {
        // Arrange
        var learners = new List<Learner>
        {
            new() { Id = 1, Ukprn = 12345, Uln = 1234567890, ApprenticeshipId = null },
            new() { Id = 2, Ukprn = 12345, Uln = 1234567891, ApprenticeshipId = 12345 },
            new() { Id = 3, Ukprn = 12345, Uln = 1234567892, ApprenticeshipId = null }
        };

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var page = 1;
        var pageSize = 100;
        var limit = 100;
        var offset = 0;
        bool excludeApproved = true;

        // Act
        var result = await _repository.GetAllLearners(page, pageSize, limit, offset, excludeApproved, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.Data.Should().OnlyContain(l => l.ApprenticeshipId == null);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
    }

    [Test]
    public async Task Then_Handles_Pagination_Correctly()
    {
        // Arrange
        var learners = new List<Learner>();
        for (int i = 1; i <= 5; i++)
        {
            learners.Add(new Learner { Id = i, Ukprn = 12345, Uln = 1234567890 + i, ApprenticeshipId = null });
        }

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var page = 2;
        var pageSize = 2;
        var limit = 2;
        var offset = 2;
        bool excludeApproved = false;

        // Act
        var result = await _repository.GetAllLearners(page, pageSize, limit, offset, excludeApproved, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(5);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
    }

    [Test]
    public async Task Then_Returns_Empty_Result_When_No_Learners_Exist()
    {
        // Arrange
        var page = 1;
        var pageSize = 100;
        var limit = 100;
        var offset = 0;
        bool excludeApproved = false;

        // Act
        var result = await _repository.GetAllLearners(page, pageSize, limit, offset, excludeApproved, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(pageSize);
    }

    [Test]
    public async Task Then_Handles_Null_PageSize_Correctly()
    {
        // Arrange
        var learners = new List<Learner>
        {
            new() { Id = 1, Ukprn = 12345, Uln = 1234567890, ApprenticeshipId = null },
            new() { Id = 2, Ukprn = 12345, Uln = 1234567891, ApprenticeshipId = null }
        };

        _dbContext.Learners.AddRange(learners);
        await _dbContext.SaveChangesAsync();

        var page = 1;
        int? pageSize = null;
        var limit = int.MaxValue;
        var offset = 0;
        bool excludeApproved = false;

        // Act
        var result = await _repository.GetAllLearners(page, pageSize, limit, offset, excludeApproved, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.Page.Should().Be(page);
        result.PageSize.Should().Be(int.MaxValue);
    }
}