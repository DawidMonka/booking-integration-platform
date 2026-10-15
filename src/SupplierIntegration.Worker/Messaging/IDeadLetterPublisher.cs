using Confluent.Kafka;

namespace SupplierIntegration.Worker.Messaging;

public interface IDeadLetterPublisher
{
    Task PublishAsync(ConsumeResult<string, string> sourceMessage, Exception exception, CancellationToken cancellationToken = default);
}