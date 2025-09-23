using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.LearnerData.Data;
using Microsoft.Extensions.Logging;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.LearnerData.Application.Commands.StopBackApprenticeship;

namespace SFA.DAS.LearnerData.UnitTests.Repositories;

[TestFixture]
public class WhenIStopBackApprenticeshipId
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
    public async Task AssignApprenticeshipId_Updates_ApprenticeshipId_When_Learner_Matches_And_Set_ApprenticeshipId_Is_Null(
        StopBackApprenticeshipCommand command,
        Learner learner)
    {
        learner.Id = command.LearnerDataId;
        learner.Uln = command.uln;
        learner.ApprenticeshipId = command.ApprenticeshipId;
        _dbContext.Learners.Add(learner);
        _dbContext.SaveChanges();

        await _repository.StopBackApprenticeshipId(command, _cancellationToken);

        var updatedLearner = await _dbContext.Learners
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Uln == command.uln, _cancellationToken);
        updatedLearner.Should().NotBeNull();
        updatedLearner.ApprenticeshipId.Should().BeNull();
    }    
}