using NServiceBus;

namespace SFA.DAS.LearnerData.Messages;

public class LearnerDataUpdatedEvent
{
    public long LearnerId { get; set; }
    public ChangeSummary ChangeSummary { get; set; } = new();
    public string EventSource { get; set; } = "SFA.DAS.LearnerData.Api";
    public Guid EventId { get; set; } = Guid.NewGuid();
} 