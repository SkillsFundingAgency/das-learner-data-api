using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.LearnerData.Api.Models.Requests;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.Api.UnitTests.Controllers.LearnersController;

public class WhenICreateLearner
{
    [Test, MoqAutoData]
    public async Task Then_Created_Result_Is_Returned(
        SaveLearnerRequest request,
        [Greedy] Api.Controllers.LearnersController sut
        )
    {
        var result = await sut.Create(request);
        result.Should().NotBeNull();
        
        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
    }
}