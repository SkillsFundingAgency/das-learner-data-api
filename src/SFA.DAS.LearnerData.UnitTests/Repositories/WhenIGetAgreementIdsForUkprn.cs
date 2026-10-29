using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Messages;

namespace SFA.DAS.LearnerData.UnitTests.Repositories;

public class WhenIGetAgreementIds
{
    private const long Ukprn = 100;
    private LearnerRepository _repository;
    private LearnerDataDbContext _dbContext;
    private CancellationToken _cancellationToken;

    [SetUp]
    public void Setup()
    {
        _dbContext = new LearnerDataDbContext(
            new DbContextOptionsBuilder<LearnerDataDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString(), b => b.EnableNullChecks(false))
                .EnableSensitiveDataLogging()
                .ConfigureWarnings(w => w.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))
                .Options);

        _repository = new LearnerRepository(_dbContext, Mock.Of<ILogger<LearnerRepository>>());
        _cancellationToken = CancellationToken.None;

        _dbContext.Learners.AddRange(
            new Learner
            {
                Ukprn = Ukprn,
                AgreementId = "AGR001",
                FirstName = "John",
                LastName = "Smith",
                Uln = 1111111111,
                StartDate = new DateTime(2026, 4, 15),
                TrainingCode = "100",
                LearningType = LearningType.Apprenticeship
            },
            new Learner
            {
                Ukprn = Ukprn,
                AgreementId = "AGR001",
                FirstName = "Jane",
                LastName = "Doe",
                Uln = 2222222222,
                StartDate = new DateTime(2026, 4, 20),
                TrainingCode = "100",
                LearningType = LearningType.Apprenticeship
            },
            new Learner
            {
                Ukprn = Ukprn,
                AgreementId = "AGR002",
                FirstName = "Amy",
                LastName = "Jones",
                Uln = 3333333333,
                StartDate = new DateTime(2027, 5, 10),
                TrainingCode = "200",
                LearningType = LearningType.ApprenticeshipUnit,
                ApprenticeshipId = 999
            },
            new Learner
            {
                Ukprn = Ukprn,
                AgreementId = "AGR003",
                FirstName = "Bob",
                LastName = "Smith",
                Uln = 4444444444,
                StartDate = new DateTime(2026, 5, 15),
                TrainingCode = "200",
                LearningType = LearningType.ApprenticeshipUnit,
            },
            new Learner
            {
                Ukprn = Ukprn,
                AgreementId = null,
                FirstName = "No",
                LastName = "Agreement",
                Uln = 5555555555,
                StartDate = new DateTime(2026, 4, 1),
                TrainingCode = "100",
                LearningType = LearningType.Apprenticeship
            },
            new Learner
            {
                Ukprn = Ukprn + 1,
                AgreementId = "OTHER",
                FirstName = "Other",
                LastName = "Provider",
                Uln = 6666666666,
                StartDate = new DateTime(2026, 4, 1),
                TrainingCode = "100",
                LearningType = LearningType.Apprenticeship
            });

        _dbContext.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task Then_Returns_Distinct_AgreementIds_For_Ukprn_And_Excludes_Null_Agreements()
    {
        // Act
        var result = await GetAgreementIds();

        // Assert
        result.Should().BeEquivalentTo("AGR001", "AGR002", "AGR003");
    }

    [Test]
    public async Task Then_Excludes_Approved_Learners_When_Requested()
    {
        // Act
        var result = await GetAgreementIds(excludeApproved: true);

        // Assert
        result.Should().BeEquivalentTo("AGR001", "AGR003");
    }

    [Test]
    public async Task Then_Excludes_Learners_After_Max_Start_Date()
    {
        // Act
        var result = await GetAgreementIds(maxStartDate: "2026-12-31");

        // Assert
        result.Should().BeEquivalentTo("AGR001", "AGR003");
    }

    [TestCase("Jan", "AGR001")]
    [TestCase("Smith", "AGR001,AGR003")]
    [TestCase("2222222222", "AGR001")]
    [TestCase("No match", "")]
    public async Task Then_Filters_By_FirstName_LastName_Or_Uln(
        string filter,
        string expectedAgreementIds)
    {
        // Act
        var result = await GetAgreementIds(filter: filter);

        // Assert
        result.Should().BeEquivalentTo(
            expectedAgreementIds.Split(',', StringSplitOptions.RemoveEmptyEntries));
    }

    [Test]
    public async Task Then_Filters_By_Start_Month()
    {
        // Act
        var result = await GetAgreementIds(startMonth: 4);

        // Assert
        result.Should().BeEquivalentTo("AGR001");
    }

    [Test]
    public async Task Then_Ignores_Invalid_Start_Month()
    {
        // Act
        var result = await GetAgreementIds(startMonth: 13);

        // Assert
        result.Should().BeEquivalentTo("AGR001", "AGR002", "AGR003");
    }

    [Test]
    public async Task Then_Filters_By_Start_Year()
    {
        // Act
        var result = await GetAgreementIds(startYear: 2027);

        // Assert
        result.Should().BeEquivalentTo("AGR002");
    }

    [Test]
    public async Task Then_Filters_By_Course_Code()
    {
        // Act
        var result = await GetAgreementIds(courseCode: "200");

        // Assert
        result.Should().BeEquivalentTo("AGR002", "AGR003");
    }

    [Test]
    public async Task Then_Filters_By_Learning_Type()
    {
        // Act
        var result = await GetAgreementIds(learningType: LearningType.Apprenticeship);

        // Assert
        result.Should().BeEquivalentTo("AGR001");
    }

    [Test]
    public async Task Then_Excludes_Learners_With_Excluded_Ulns()
    {
        // Act
        var result = await GetAgreementIds(
            excludeUlns: [1111111111, 2222222222]);

        // Assert
        result.Should().BeEquivalentTo("AGR002", "AGR003");
    }

    private Task<List<string>> GetAgreementIds(
        string filter = "",
        bool excludeApproved = false,
        int? startMonth = null,
        int startYear = 0,
        string maxStartDate = "",
        List<long>? excludeUlns = null,
        string courseCode = "",
        LearningType? learningType = null)
    {
        return _repository.GetAgreementIds(
            Ukprn,
            filter,
            excludeApproved,
            startMonth,
            startYear,
            maxStartDate,
            excludeUlns ?? new List<long>(),
            courseCode,
            learningType,
            _cancellationToken);
    }
}