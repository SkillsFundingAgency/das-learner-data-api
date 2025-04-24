using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands.SaveLearner;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Commands;

public class SaveLearnerCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Exists(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        SaveLearnerCommandHandler sut
    )
    {
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Updated };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();

        var result = await sut.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Result.Should().Be(response.Result);
        
        repository.Verify();
    }
    
    [Test, MoqAutoData]
    public async Task Handle_Save_When_Learner_Does_Not_Exist(
        SaveLearnerCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        SaveLearnerCommandHandler sut
    )
    {
        var response = new SaveLearnerCommandResponse { Id = 1, Result = SaveLearnerResult.Created };
        
        repository
            .Setup(x => x.Save(command, It.IsAny<CancellationToken>())).ReturnsAsync(response)
            .Verifiable();

        var result = await sut.Handle(command, CancellationToken.None);
        result.Should().NotBeNull();
        result.Id.Should().Be(response.Id);
        result.Result.Should().Be(response.Result);
        
        repository.Verify();
    }
}