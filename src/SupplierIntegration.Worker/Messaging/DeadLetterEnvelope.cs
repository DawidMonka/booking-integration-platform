namespace SupplierIntegration.Worker.Messaging;

public sealed class DeadLetterEnvelope(string SourceTopic, int SourcePartition, long SourceOffset,
    string? sourceKey, string? sourceValue, string ErrorType, string ErrorMessage, DateTime FailedAtUtc)
{
    public string SourceTopic { get; set; } = SourceTopic;
    public int SourcePartition { get; set; } = SourcePartition;
    public long SourceOffset { get; set; } = SourceOffset;
    public string? SourceKey { get; set; } = sourceKey;
    public string? SourceValue { get; set; } = sourceValue;
    public string ErrorType { get; set; } = ErrorType;
    public string ErrorMessage { get; set; } = ErrorMessage;
    public DateTime FailedAtUtc { get; set; } = FailedAtUtc;
}