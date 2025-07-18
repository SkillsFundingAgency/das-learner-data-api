using System.Text.Json;
using System.Text.Json.Serialization;
using NServiceBus;

namespace SFA.DAS.LearnerData.Messages;

public class ChangeSummary
{
    public List<IChange> Changes { get; init; } = new();
    public bool HasChanges => Changes.Count > 0;
}

public enum ChangeType
{
    FirstNameChange,
    LastNameChange,
    EmailChange,
    DobChange,
    StartDateChange,
    PlannedEndDateChange,
    EpaoPriceChange,
    TrainingPriceChange
}

[JsonConverter(typeof(ChangeJsonConverter))]
public interface IChange
{
    ChangeType ChangeType { get; }
}

public class FirstNameChange : IChange
{
    public ChangeType ChangeType => ChangeType.FirstNameChange;
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
}

public class LastNameChange : IChange
{
    public ChangeType ChangeType => ChangeType.LastNameChange;
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
}

public class EmailChange : IChange
{
    public ChangeType ChangeType => ChangeType.EmailChange;
    public string? OldValue { get; init; }
    public string? NewValue { get; init; }
}

public class DobChange : IChange
{
    public ChangeType ChangeType => ChangeType.DobChange;
    public DateTime? OldValue { get; init; }
    public DateTime? NewValue { get; init; }
}

public class StartDateChange : IChange
{
    public ChangeType ChangeType => ChangeType.StartDateChange;
    public DateTime? OldValue { get; init; }
    public DateTime? NewValue { get; init; }
}

public class PlannedEndDateChange : IChange
{
    public ChangeType ChangeType => ChangeType.PlannedEndDateChange;
    public DateTime? OldValue { get; init; }
    public DateTime? NewValue { get; init; }
}

public class EpaoPriceChange : IChange
{
    public ChangeType ChangeType => ChangeType.EpaoPriceChange;
    public int? OldValue { get; init; }
    public int? NewValue { get; init; }
}

public class TrainingPriceChange : IChange
{
    public ChangeType ChangeType => ChangeType.TrainingPriceChange;
    public int? OldValue { get; init; }
    public int? NewValue { get; init; }
}

public class ChangeJsonConverter : JsonConverter<IChange>
{
    public override IChange? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();

        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;

        if (!root.TryGetProperty("ChangeType", out var changeTypeElement))
            throw new JsonException("Missing ChangeType property");

        var changeType = JsonSerializer.Deserialize<ChangeType>(changeTypeElement.GetRawText(), options);

        return changeType switch
        {
            ChangeType.FirstNameChange => JsonSerializer.Deserialize<FirstNameChange>(root.GetRawText(), options),
            ChangeType.LastNameChange => JsonSerializer.Deserialize<LastNameChange>(root.GetRawText(), options),
            ChangeType.EmailChange => JsonSerializer.Deserialize<EmailChange>(root.GetRawText(), options),
            ChangeType.DobChange => JsonSerializer.Deserialize<DobChange>(root.GetRawText(), options),
            ChangeType.StartDateChange => JsonSerializer.Deserialize<StartDateChange>(root.GetRawText(), options),
            ChangeType.PlannedEndDateChange => JsonSerializer.Deserialize<PlannedEndDateChange>(root.GetRawText(), options),
            ChangeType.EpaoPriceChange => JsonSerializer.Deserialize<EpaoPriceChange>(root.GetRawText(), options),
            ChangeType.TrainingPriceChange => JsonSerializer.Deserialize<TrainingPriceChange>(root.GetRawText(), options),
            _ => throw new JsonException($"Unknown change type: {changeType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, IChange value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
} 