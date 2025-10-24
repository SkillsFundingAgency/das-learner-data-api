using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Repositories;

[TestFixture]
public class WhenISaveLearnerData
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
    public async Task SaveLearnerData_When_Learner_Is_New(
        SaveLearnerCommand command)
    {
        await _repository.Save(command, _cancellationToken);

        var newLearner = await _dbContext.Learners
            .AsNoTracking()
            .FirstOrDefaultAsync();

        newLearner.Should().NotBeNull();
        newLearner.Uln.Should().Be(command.Uln);
        newLearner.Ukprn.Should().Be(command.Ukprn);
        newLearner.FirstName.Should().Be(command.FirstName);
        newLearner.LastName.Should().Be(command.LastName);
        newLearner.Email.Should().Be(command.Email);
        newLearner.Dob.Should().Be(command.Dob);
        newLearner.AcademicYear.Should().Be(command.AcademicYear);
        newLearner.StartDate.Should().Be(command.StartDate);
        newLearner.PlannedEndDate.Should().Be(command.PlannedEndDate);
        newLearner.PercentageLearningToBeDelivered.Should().Be(command.PercentageLearningToBeDelivered);
        newLearner.EpaoPrice.Should().Be(command.EpaoPrice);
        newLearner.TrainingPrice.Should().Be(command.TrainingPrice);
        newLearner.AgreementId.Should().Be(command.AgreementId);
        newLearner.StandardCode.Should().Be(command.StandardCode);
        newLearner.IsFlexiJob.Should().Be(command.IsFlexiJob);
        newLearner.PlannedOTJTrainingHours.Should().Be(command.PlannedOTJTrainingHours);
        newLearner.ReceivedDate.Should().Be(command.ReceivedDate);
        newLearner.CorrelationId.Should().Be(command.CorrelationId);
        newLearner.ConsumerReference.Should().Be(command.ConsumerReference);
    }

    [Test, MoqAutoData]
    public async Task SaveLearnerData_When_Learner_Updating_existing_record(
        SaveLearnerCommand command,
        Learner existinglearner)
    {
        existinglearner.Ukprn = command.Ukprn;
        existinglearner.Uln = command.Uln;
        existinglearner.StandardCode = command.StandardCode;
        existinglearner.AcademicYear = command.AcademicYear;
        existinglearner.ApprenticeshipId = null;
        _dbContext.Learners.Add(existinglearner);
        _dbContext.SaveChanges();

        await _repository.Save(command, _cancellationToken);

        var learner = await _dbContext.Learners
            .AsNoTracking()
            .FirstOrDefaultAsync();

        learner.Should().NotBeNull();
        learner.Uln.Should().Be(command.Uln);
        learner.Ukprn.Should().Be(command.Ukprn);
        learner.FirstName.Should().Be(command.FirstName);
        learner.LastName.Should().Be(command.LastName);
        learner.Email.Should().Be(command.Email);
        learner.Dob.Should().Be(command.Dob);
        learner.AcademicYear.Should().Be(command.AcademicYear);
        learner.StartDate.Should().Be(command.StartDate);
        learner.PlannedEndDate.Should().Be(command.PlannedEndDate);
        learner.PercentageLearningToBeDelivered.Should().Be(command.PercentageLearningToBeDelivered);
        learner.EpaoPrice.Should().Be(command.EpaoPrice);
        learner.TrainingPrice.Should().Be(command.TrainingPrice);
        learner.AgreementId.Should().Be(command.AgreementId);
        learner.StandardCode.Should().Be(command.StandardCode);
        learner.IsFlexiJob.Should().Be(command.IsFlexiJob);
        learner.PlannedOTJTrainingHours.Should().Be(command.PlannedOTJTrainingHours);
        learner.ReceivedDate.Should().Be(command.ReceivedDate);
        learner.CorrelationId.Should().Be(command.CorrelationId);
        learner.ConsumerReference.Should().Be(command.ConsumerReference);
    }

    [Test, MoqAutoData]
    public async Task AssignApprenticeshipId_Throws_Exception_When_Learner_Not_Found(
        SaveLearnerCommand command,
        Learner existinglearner)
    {
        existinglearner.Ukprn = command.Ukprn;
        existinglearner.Uln = command.Uln;
        existinglearner.StandardCode = command.StandardCode;
        existinglearner.AcademicYear = command.AcademicYear;
        existinglearner.ApprenticeshipId = 12345;
        _dbContext.Learners.Add(existinglearner);
        _dbContext.SaveChanges();

        var ex = () => _repository.Save(command, _cancellationToken);

        await ex.Should().ThrowAsync<InvalidOperationException>().WithMessage($"Learner with ID {existinglearner.Id} already has ApprenticeshipId {existinglearner.ApprenticeshipId} assigned. Cannot update.");
    }
}