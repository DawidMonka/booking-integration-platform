using System.Text.Json;
using Confluent.Kafka;

namespace SupplierIntegration.Worker.Messaging;

public sealed class KafkaDeadLetterPublisher : IDeadLetterPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly string _deadLetterTopic;

    public KafkaDeadLetterPublisher(IConfiguration configuration)
    {
        var bootstrapServers = configuration.GetValue<string>("Kafka:BootstrapServers") ??
            throw new InvalidOperationException("Kafka bootstrap servers configuration is missing.");

        _deadLetterTopic = configuration.GetValue<string>("Kafka:DeadLetterTopic") ??
            throw new InvalidOperationException("Kafka dead letter topic configuration is missing.");

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync(ConsumeResult<string, string> sourceMessage, Exception exception, CancellationToken cancellationToken = default)
    {
        var envelope = new DeadLetterEnvelope(
            sourceMessage.Topic,
            sourceMessage.Partition.Value,
            sourceMessage.Offset.Value,
            sourceMessage.Message.Key,
            sourceMessage.Message.Value,
            exception.GetType().Name,
            exception.Message,
            DateTime.UtcNow);

        var deadLetterKey = $"{sourceMessage.Topic}-{sourceMessage.Partition}-{sourceMessage.Offset}";

        var message = new Message<string, string>
        {
            Key = deadLetterKey,
            Value = JsonSerializer.Serialize(envelope)
        };

        await _producer.ProduceAsync(_deadLetterTopic, message, cancellationToken); 
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}