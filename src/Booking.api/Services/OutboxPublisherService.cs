using BookingPlatform.Api.Messaging;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Api.Services;

public sealed class OutboxPublisherService(
    IServiceScopeFactory scopeFactory, 
    IEventPublisher eventPublisher, 
    ILogger<OutboxPublisherService> logger) : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(5);
    private const int BatchSize = 20;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Outbox publisher started");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await PublishPendingMessageAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing Outbox messages.");
            }

            try
            {
                await Task.Delay(PollingInterval, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task PublishPendingMessageAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

        var messages = await dbContext.OutboxMessages
        .Where(message => message.PublishedAtUtc == null)
        .OrderBy(message => message.OccurredAtUtc)
        .Take(BatchSize)
        .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                await eventPublisher.PublishAsync(
                    message.EventType,
                    message.AggregateId.ToString(),
                    message.Payload,
                    cancellationToken);

                message.MarkPublished();
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogInformation("Published Outbox message {OutboxMessageId} for booking {BookingId} to topic {Topic}.",
                    message.Id,
                    message.AggregateId,
                    message.EventType);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                message.MarkFailed(ex.Message);
                await dbContext.SaveChangesAsync(cancellationToken);

                logger.LogError(ex, "Failed to publish Outbox message {OutboxMessageId}, Attempt: {AttemptCount}.",
                message.Id, message.AttemptCount);
            }
        }
    }
}