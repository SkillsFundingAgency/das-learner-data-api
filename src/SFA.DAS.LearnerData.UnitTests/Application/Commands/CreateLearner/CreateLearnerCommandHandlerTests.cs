using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.LearnerData.Application.Commands;
using SFA.DAS.LearnerData.Data.Entities;
using SFA.DAS.LearnerData.Data.Repositories;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.LearnerData.UnitTests.Application.Commands.CreateLearner;

public class CreateLearnerCommandHandlerTests
{
    [Test, MoqAutoData]
    public async Task Handle_Create_Learner(
        CreateLearnerCommand request,
        [Frozen] Mock<ILearnerDataRepository> repository,
        CreateLearnerCommandHandler sut
    )
    {
        await sut.Handle(request, CancellationToken.None);

        repository.Verify(x => x.Create(It.Is<Learner>(learner =>
                learner.Uln == request.Uln &&
                learner.Ukprn == request.Ukprn &&
                learner.FirstName == request.FirstName &&
                learner.LastName == request.LastName &&
                learner.Email == request.Email &&
                learner.Dob == request.Dob &&
                learner.AcademicYear == request.AcademicYear &&
                learner.StartDate == request.StartDate &&
                learner.PlannedEndDate == request.PlannedEndDate &&
                learner.PercentageLearningToBeDelivered == request.PercentageLearningToBeDelivered &&
                learner.EpaoPrice == request.EpaoPrice &&
                learner.TrainingPrice == request.TrainingPrice &&
                learner.AgreementId == request.AgreementId &&
                learner.ConsumerReference == request.ConsumerReference &&
                learner.CorrelationId == request.CorrelationId &&
                learner.ReceivedDate == request.ReceivedDate &&
                learner.StandardCode == request.StandardCode &&
                learner.IsFlexiJob == request.IsFlexiJob &&
                learner.PlannedOTJTrainingHours == request.PlannedOTJTrainingHours))
            , Times.Once);
    }
}