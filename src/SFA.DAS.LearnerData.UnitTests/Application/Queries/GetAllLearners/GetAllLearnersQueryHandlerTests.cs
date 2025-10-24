using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Queries.GetAllLearners;
using SFA.DAS.LearnerData.Data;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Queries.GetAllLearners;

public class GetAllLearnersQueryHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_Returns_Successful_Result_When_Learners_Exist(
        GetAllLearnersQuery query,
        List<Learner> learners,
        [Frozen] Mock<ILearnerRepository> repository,
        GetAllLearnersQueryHandler sut
    )
    {
        // Arrange
        var pagedResult = new PagedResult<Learner>
        {
            Data = learners,
            TotalItems = learners.Count,
            TotalPages = 1,
            PageSize = query.PageSize ?? 100,
            Page = query.Page
        };

        repository.Setup(x => x.GetAllLearners(
            query.Page,
            query.PageSize,
            query.Limit,
            query.Offset,
            query.ExcludeApproved,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult)
            .Verifiable();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(learners.Count);
        result.TotalItems.Should().Be(learners.Count);
        result.TotalPages.Should().Be(1);
        result.PageSize.Should().Be(query.PageSize ?? 100);
        result.Page.Should().Be(query.Page);

        var firstResultItem = result.Items.First();
        var firstLearner = learners.First();
        firstResultItem.Id.Should().Be(firstLearner.Id);
        firstResultItem.Uln.Should().Be(firstLearner.Uln);
        firstResultItem.Ukprn.Should().Be(firstLearner.Ukprn);
        firstResultItem.FirstName.Should().Be(firstLearner.FirstName);
        firstResultItem.LastName.Should().Be(firstLearner.LastName);
        firstResultItem.Email.Should().Be(firstLearner.Email);
        firstResultItem.Dob.Should().Be(firstLearner.Dob);
        firstResultItem.AcademicYear.Should().Be(firstLearner.AcademicYear);
        firstResultItem.StartDate.Should().Be(firstLearner.StartDate);
        firstResultItem.PlannedEndDate.Should().Be(firstLearner.PlannedEndDate);
        firstResultItem.PercentageLearningToBeDelivered.Should().Be(firstLearner.PercentageLearningToBeDelivered);
        firstResultItem.EpaoPrice.Should().Be(firstLearner.EpaoPrice);
        firstResultItem.TrainingPrice.Should().Be(firstLearner.TrainingPrice);
        firstResultItem.AgreementId.Should().Be(firstLearner.AgreementId);
        firstResultItem.ConsumerReference.Should().Be(firstLearner.ConsumerReference);
        firstResultItem.CorrelationId.Should().Be(firstLearner.CorrelationId);
        firstResultItem.ReceivedDate.Should().Be(firstLearner.ReceivedDate);
        firstResultItem.StandardCode.Should().Be(firstLearner.StandardCode);
        firstResultItem.IsFlexiJob.Should().Be(firstLearner.IsFlexiJob);
        firstResultItem.PlannedOTJTrainingHours.Should().Be(firstLearner.PlannedOTJTrainingHours);

        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Returns_Empty_Result_When_No_Learners_Exist(
        GetAllLearnersQuery query,
        [Frozen] Mock<ILearnerRepository> repository,
        GetAllLearnersQueryHandler sut
    )
    {
        // Arrange
        var emptyPagedResult = new PagedResult<Learner>
        {
            Data = new List<Learner>(),
            TotalItems = 0,
            TotalPages = 0,
            PageSize = query.PageSize ?? 100,
            Page = query.Page
        };

        repository.Setup(x => x.GetAllLearners(
            query.Page,
            query.PageSize,
            query.Limit,
            query.Offset,
            query.ExcludeApproved,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyPagedResult)
            .Verifiable();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.PageSize.Should().Be(query.PageSize ?? 100);
        result.Page.Should().Be(query.Page);

        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Handles_ExcludeApproved_True_Correctly(
        GetAllLearnersQuery query,
        List<Learner> learners,
        [Frozen] Mock<ILearnerRepository> repository,
        GetAllLearnersQueryHandler sut
    )
    {
        // Arrange
        query = new GetAllLearnersQuery(query.Page, query.PageSize, true);
        var pagedResult = new PagedResult<Learner>
        {
            Data = learners,
            TotalItems = learners.Count,
            TotalPages = 1,
            PageSize = query.PageSize ?? 100,
            Page = query.Page
        };

        repository.Setup(x => x.GetAllLearners(
            query.Page,
            query.PageSize,
            query.Limit,
            query.Offset,
            true,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult)
            .Verifiable();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(learners.Count);

        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Handles_ExcludeApproved_False_Correctly(
        GetAllLearnersQuery query,
        List<Learner> learners,
        [Frozen] Mock<ILearnerRepository> repository,
        GetAllLearnersQueryHandler sut
    )
    {
        // Arrange
        query = new GetAllLearnersQuery(query.Page, query.PageSize, false);
        var pagedResult = new PagedResult<Learner>
        {
            Data = learners,
            TotalItems = learners.Count,
            TotalPages = 1,
            PageSize = query.PageSize ?? 100,
            Page = query.Page
        };

        repository.Setup(x => x.GetAllLearners(
            query.Page,
            query.PageSize,
            query.Limit,
            query.Offset,
            false,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult)
            .Verifiable();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(learners.Count);

        repository.Verify();
    }

    [Test, MoqAutoData]
    public async Task Handle_Handles_Pagination_Correctly(
        GetAllLearnersQuery query,
        List<Learner> learners,
        [Frozen] Mock<ILearnerRepository> repository,
        GetAllLearnersQueryHandler sut
    )
    {
        // Arrange
        query = query with { Page = 2, PageSize = 50 };
        var pagedResult = new PagedResult<Learner>
        {
            Data = learners,
            TotalItems = 150,
            TotalPages = 3,
            PageSize = 50,
            Page = 2
        };

        repository.Setup(x => x.GetAllLearners(
            2,
            50,
            query.Limit,
            query.Offset,
            query.ExcludeApproved,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResult)
            .Verifiable();

        // Act
        var result = await sut.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(50);
        result.TotalItems.Should().Be(150);
        result.TotalPages.Should().Be(3);

        repository.Verify();
    }
}
