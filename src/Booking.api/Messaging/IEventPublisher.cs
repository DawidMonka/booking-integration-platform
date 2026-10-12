namespace BookingPlatform.Api.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<T>(string topic, T message, CancellationToken cancellationToken = default);   
}
