using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Responses;
using SFA.DAS.LearnerData.Application.Queries.GetAllLearners;
using SFA.DAS.LearnerData.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenIGetAllLearners
{
    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_Learners_Exist(
        int page,
        int pageSize,
        bool excludeApproved,
        GetAllLearnersResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<IPagedLinkHeaderService> pagedLinkHeaderService,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        // Arrange
        var pageLinks = new KeyValuePair<string, StringValues>("Link", new StringValues("Link: <https://api.example.com/learners?page=2>; rel=\"next\""));
        pagedLinkHeaderService.Setup(x => x.GetPageLinks(It.IsAny<GetAllLearnersQuery>(), queryResult))
            .Returns(pageLinks);

        sender.Setup(x => x.Send(It.Is<GetAllLearnersQuery>(q => 
            q.Page == page && 
            q.PageSize == pageSize && 
            q.ExcludeApproved == excludeApproved), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();

        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        // Act
        var result = await sut.GetAllLearners(page, pageSize, excludeApproved);

        // Assert
        result.Should().NotBeNull();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult.Value as GetAllLearnersResponse;
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(queryResult.Items.Count());
        response.TotalItems.Should().Be(queryResult.TotalItems);
        response.TotalPages.Should().Be(queryResult.TotalPages);
        response.PageSize.Should().Be(queryResult.PageSize);
        response.Page.Should().Be(queryResult.Page);

        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_With_Default_Parameters(
        GetAllLearnersResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<IPagedLinkHeaderService> pagedLinkHeaderService,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        // Arrange
        var pageLinks = new KeyValuePair<string, StringValues>("Link", new StringValues("Link: <https://api.example.com/learners?page=2>; rel=\"next\""));
        pagedLinkHeaderService.Setup(x => x.GetPageLinks(It.IsAny<GetAllLearnersQuery>(), queryResult))
            .Returns(pageLinks);

        sender.Setup(x => x.Send(It.Is<GetAllLearnersQuery>(q => 
            q.Page == 1 && 
            q.PageSize == 100 && 
            q.ExcludeApproved == true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();

        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        // Act
        var result = await sut.GetAllLearners();

        // Assert
        result.Should().NotBeNull();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult.Value as GetAllLearnersResponse;
        response.Should().NotBeNull();

        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_BadRequest_Response_Is_Returned_When_PageSize_Exceeds_1000(
        int page,
        int pageSize,
        bool excludeApproved,
        [Frozen] Mock<ISender> sender,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        // Arrange
        pageSize = 1001;

        // Act
        var result = await sut.GetAllLearners(page, pageSize, excludeApproved);

        // Assert
        result.Should().NotBeNull();
        var badRequestResult = result as BadRequestResult;
        badRequestResult.Should().NotBeNull();
        
        sender.Verify(x => x.Send(It.IsAny<GetAllLearnersQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test, MoqAutoData]
    public async Task Then_Ok_Response_Is_Returned_When_PageSize_Is_Exactly_1000(
        int page,
        bool excludeApproved,
        GetAllLearnersResult queryResult,
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<IPagedLinkHeaderService> pagedLinkHeaderService,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        // Arrange
        var pageSize = 1000;
        var pageLinks = new KeyValuePair<string, StringValues>("Link", new StringValues("Link: <https://api.example.com/learners?page=2>; rel=\"next\""));
        pagedLinkHeaderService.Setup(x => x.GetPageLinks(It.IsAny<GetAllLearnersQuery>(), queryResult))
            .Returns(pageLinks);

        sender.Setup(x => x.Send(It.Is<GetAllLearnersQuery>(q => 
            q.Page == page && 
            q.PageSize == pageSize && 
            q.ExcludeApproved == excludeApproved), It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult)
            .Verifiable();

        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        // Act
        var result = await sut.GetAllLearners(page, pageSize, excludeApproved);

        // Assert
        result.Should().NotBeNull();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult.Value as GetAllLearnersResponse;
        response.Should().NotBeNull();

        sender.Verify();
    }

    [Test, MoqAutoData]
    public async Task Then_Empty_Response_Is_Returned_When_No_Learners_Exist(
        int page,
        int pageSize,
        bool excludeApproved,
        [Frozen] Mock<ISender> sender,
        [Frozen] Mock<IPagedLinkHeaderService> pagedLinkHeaderService,
        [Greedy] Api.Controllers.LearnersController sut
    )
    {
        // Arrange
        var emptyResult = new GetAllLearnersResult
        {
            Items = [],
            TotalItems = 0,
            PageSize = pageSize,
            Page = page
        };

        var pageLinks = new KeyValuePair<string, StringValues>("Link", new StringValues(""));
        pagedLinkHeaderService.Setup(x => x.GetPageLinks(It.IsAny<GetAllLearnersQuery>(), emptyResult))
            .Returns(pageLinks);

        sender.Setup(x => x.Send(It.Is<GetAllLearnersQuery>(q => 
            q.Page == page && 
            q.PageSize == pageSize && 
            q.ExcludeApproved == excludeApproved), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyResult)
            .Verifiable();

        sut.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };

        // Act
        var result = await sut.GetAllLearners(page, pageSize, excludeApproved);

        // Assert
        result.Should().NotBeNull();
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        
        var response = okResult.Value as GetAllLearnersResponse;
        response.Should().NotBeNull();
        response.Data.Should().BeEmpty();
        response.TotalItems.Should().Be(0);
        response.TotalPages.Should().Be(0);

        sender.Verify();
    }
}
