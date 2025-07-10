using NServiceBus;

namespace SFA.DAS.LearnerData.Messages;

public class ChangeSummary
{
    public List<FieldChange> Changes { get; init; } = new();
    public bool HasChanges => Changes.Count > 0;
}

public class FieldChange
{
    public string FieldName { get; set; } = string.Empty;
    public object? OldValue { get; set; }
    public object? NewValue { get; set; }
} 