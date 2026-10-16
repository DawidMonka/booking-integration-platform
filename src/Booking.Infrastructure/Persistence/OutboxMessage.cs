namespace BookingPlatform.Infrastructure.Persistence;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
        
    }

    public Guid Id { get; private set; }
    public Guid AggregateId { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTimeOffset OccurredAtUtc { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public int AttemptCount { get; private set; }
    public string? LastError { get; private set; }

    public OutboxMessage(Guid aggregateId, string eventType, string payload)
    {
        if (aggregateId == Guid.Empty)
        {
            throw new ArgumentException("Aggregate ID cannot be empty", nameof(aggregateId));
        }

        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException("Payload cannot be empty", nameof(payload));
        }

        Id = Guid.NewGuid();
        AggregateId = aggregateId;
        EventType = eventType;
        Payload = payload;
        OccurredAtUtc = DateTimeOffset.UtcNow;
    }

    public void MarkPublished()
    {
        PublishedAtUtc = DateTimeOffset.UtcNow;
        LastError = null;
    }

    public void MarkFailed(string error)
    {
        AttemptCount++;
        LastError = error;
    }
}