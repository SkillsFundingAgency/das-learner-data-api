using SFA.DAS.LearnerData.Application.Commands;
using SFA.DAS.LearnerData.Application.Commands.CreateLearner;

namespace SFA.DAS.LearnerData.Data.Entities;

public class Learner
{
    public long Id { get; set; }
    public long Uln { get; set; }
    public long Ukprn { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime Dob { get; set; }
    public int AcademicYear { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime PlannedEndDate { get; set; }
    public int PercentageLearningToBeDelivered { get; set; }
    public int EpaoPrice { get; set; }
    public int TrainingPrice { get; set; }
    public string AgreementId { get; set; }
    public int StandardCode { get; set; }
    public bool IsFlexiJob { get; set; }
    public int PlannedOTJTrainingHours { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedDate { get; set; }
    public DateTime ReceivedDate { get; set; }
    public string CorrelationId { get; set; }
    public string ConsumerReference { get; set; }

    public static Learner From(CreateLearnerCommand command)
    {
        return new Learner
        {
            Uln = command.Uln,
            Ukprn = command.Ukprn,
            FirstName = command.FirstName,
            LastName = command.LastName,
            Email = command.Email,
            Dob = command.Dob,
            AcademicYear = command.AcademicYear,
            StartDate = command.StartDate,
            PlannedEndDate = command.PlannedEndDate,
            PercentageLearningToBeDelivered = command.PercentageLearningToBeDelivered,
            EpaoPrice = command.EpaoPrice,
            TrainingPrice = command.TrainingPrice,
            AgreementId = command.AgreementId,
            ConsumerReference = command.ConsumerReference,
            CorrelationId = command.CorrelationId,
            ReceivedDate = command.ReceivedDate,
            StandardCode = command.StandardCode,
            IsFlexiJob = command.IsFlexiJob,
            PlannedOTJTrainingHours = command.PlannedOTJTrainingHours
        };
    }
}