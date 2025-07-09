using NServiceBus;

namespace SFA.DAS.LearnerData.Messages;

public class ChangeSummary : IMessage
{
    public List<FieldChange> Changes { get; set; } = new();
    public bool HasChanges => Changes.Count > 0;
}

public class FieldChange
{
    public string FieldName { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
} 