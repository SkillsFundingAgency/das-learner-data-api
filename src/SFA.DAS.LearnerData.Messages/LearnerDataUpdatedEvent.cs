namespace SFA.DAS.LearnerData.Messages;

public class LearnerDataUpdatedEvent
{
    public long LearnerId { get; set; }
    public ChangeSummary ChangeSummary { get; set; } = new();
} 