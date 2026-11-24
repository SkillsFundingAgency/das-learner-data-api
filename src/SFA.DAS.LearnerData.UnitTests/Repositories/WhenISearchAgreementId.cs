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
public class WhenISearchAgreementId
{
    private LearnerRepository _repository;
    private LearnerDataDbContext _dbContext;
    private CancellationToken _cancellationToken;
    private const string AgreementId = "ZYTH87";

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
    public async Task Then_Filters_Result_By_AgreementId()
    {
        _dbContext.Learners.AddRange(
            CreateLearner("agreement1"),
            CreateLearner("agreement2"),
            CreateLearner(AgreementId));
        await _dbContext.SaveChangesAsync();

        var result = await _repository.Search(
            888001, 1, 100, 100, 0,
            nameof(Learner.StartDate), true, "", false,
            null, 0, "",
            [],
            null, null, AgreementId,
            _cancellationToken);

        result.Data.Should().ContainSingle(l => l.AgreementId == AgreementId);
        result.TotalItems.Should().Be(1);
    }

    private static Learner CreateLearner(string agreementId) =>
        new()
        {
            Ukprn = 888001,
            Uln = 1000000001,
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
            AgreementId = agreementId
        };
}
