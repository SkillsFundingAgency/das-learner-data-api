using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands.AssignApprenticeshipId;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Commands;

public class AssignApprenticeshipIdCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_AssignApprenticeshipId(
        AssignApprenticeshipIdCommand command,
        [Frozen] Mock<ILearnerRepository> repository,
        AssignApprenticeshipIdCommandHandler sut
    )
    {
        repository
            .Setup(x => x.AssignApprenticeshipId(command, It.IsAny<CancellationToken>()))
            .Verifiable();

        await sut.Handle(command, CancellationToken.None);
        
        repository.Verify();
    }
}