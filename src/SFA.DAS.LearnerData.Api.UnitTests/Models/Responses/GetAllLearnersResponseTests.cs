using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries.GetAllLearners;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Models.Responses;

public class GetAllLearnersResponseTests
{
    [Test, MoqAutoData]
    public void MapFrom_Returns_Correctly_Mapped_Response_When_Result_Has_Items(
        List<GetAllLearnersResultItem> resultItems
    )
    {
        // Arrange
        var result = new GetAllLearnersResult
        {
            Items = resultItems,
            TotalItems = resultItems.Count,
            PageSize = 100,
            Page = 1
        };

        // Act
        var response = GetAllLearnersResponse.MapFrom(result);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(resultItems.Count);
        response.TotalItems.Should().Be(result.TotalItems);
        response.TotalPages.Should().Be(1);
        response.PageSize.Should().Be(result.PageSize);
        response.Page.Should().Be(result.Page);

        var firstResponseItem = response.Data.First();
        var firstResultItem = resultItems.First();
        
        firstResponseItem.Id.Should().Be(firstResultItem.Id);
        firstResponseItem.Uln.Should().Be(firstResultItem.Uln);
        firstResponseItem.Ukprn.Should().Be(firstResultItem.Ukprn);
        firstResponseItem.FirstName.Should().Be(firstResultItem.FirstName);
        firstResponseItem.LastName.Should().Be(firstResultItem.LastName);
        firstResponseItem.Email.Should().Be(firstResultItem.Email);
        firstResponseItem.Dob.Should().Be(firstResultItem.Dob);
        firstResponseItem.AcademicYear.Should().Be(firstResultItem.AcademicYear);
        firstResponseItem.StartDate.Should().Be(firstResultItem.StartDate);
        firstResponseItem.PlannedEndDate.Should().Be(firstResultItem.PlannedEndDate);
        firstResponseItem.PercentageLearningToBeDelivered.Should().Be(firstResultItem.PercentageLearningToBeDelivered);
        firstResponseItem.EpaoPrice.Should().Be(firstResultItem.EpaoPrice);
        firstResponseItem.TrainingPrice.Should().Be(firstResultItem.TrainingPrice);
        firstResponseItem.AgreementId.Should().Be(firstResultItem.AgreementId);
        firstResponseItem.ConsumerReference.Should().Be(firstResultItem.ConsumerReference);
        firstResponseItem.CorrelationId.Should().Be(firstResultItem.CorrelationId);
        firstResponseItem.ReceivedDate.Should().Be(firstResultItem.ReceivedDate);
        firstResponseItem.TrainingCode.Should().Be(firstResultItem.TrainingCode);
        firstResponseItem.IsFlexiJob.Should().Be(firstResultItem.IsFlexiJob);
        firstResponseItem.PlannedOTJTrainingHours.Should().Be(firstResultItem.PlannedOTJTrainingHours);
    }

    [Test, MoqAutoData]
    public void MapFrom_Returns_Empty_Response_When_Result_Has_No_Items()
    {
        // Arrange
        var result = new GetAllLearnersResult
        {
            Items = [],
            TotalItems = 0,
            PageSize = 100,
            Page = 1
        };

        // Act
        var response = GetAllLearnersResponse.MapFrom(result);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().BeEmpty();
        response.TotalItems.Should().Be(0);
        response.TotalPages.Should().Be(0);
        response.PageSize.Should().Be(100);
        response.Page.Should().Be(1);
    }

    [Test, MoqAutoData]
    public void MapFrom_Handles_Pagination_Properties_Correctly(
        List<GetAllLearnersResultItem> resultItems
    )
    {
        // Arrange
        var result = new GetAllLearnersResult
        {
            Items = resultItems,
            TotalItems = 250,
            PageSize = 50,
            Page = 3
        };

        // Act
        var response = GetAllLearnersResponse.MapFrom(result);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(resultItems.Count);
        response.TotalItems.Should().Be(250);
        response.TotalPages.Should().Be(5);
        response.PageSize.Should().Be(50);
        response.Page.Should().Be(3);
    }

    [Test, MoqAutoData]
    public void MapFrom_Handles_Null_Values_Correctly(
        GetAllLearnersResultItem resultItem
    )
    {
        // Arrange
        resultItem.Email = null;
        resultItem.AgreementId = null;
        resultItem.PercentageLearningToBeDelivered = null;
        
        var result = new GetAllLearnersResult
        {
            Items = [resultItem],
            TotalItems = 1,
            PageSize = 100,
            Page = 1
        };

        // Act
        var response = GetAllLearnersResponse.MapFrom(result);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(1);
        
        var responseItem = response.Data.First();
        responseItem.Email.Should().BeNull();
        responseItem.AgreementId.Should().BeNull();
        responseItem.PercentageLearningToBeDelivered.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void MapFrom_Handles_Multiple_Items_Correctly(
        List<GetAllLearnersResultItem> resultItems
    )
    {
        // Arrange
        var result = new GetAllLearnersResult
        {
            Items = resultItems,
            TotalItems = resultItems.Count,
            PageSize = 100,
            Page = 1
        };

        // Act
        var response = GetAllLearnersResponse.MapFrom(result);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(resultItems.Count);
        
        var responseItems = response.Data.ToList();
        var resultItemsList = resultItems.ToList();
        
        for (int i = 0; i < resultItems.Count; i++)
        {
            responseItems[i].Id.Should().Be(resultItemsList[i].Id);
            responseItems[i].Uln.Should().Be(resultItemsList[i].Uln);
            responseItems[i].Ukprn.Should().Be(resultItemsList[i].Ukprn);
            responseItems[i].FirstName.Should().Be(resultItemsList[i].FirstName);
            responseItems[i].LastName.Should().Be(resultItemsList[i].LastName);
        }
    }
}
