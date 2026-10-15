using System.Text.Json;
using BookingPlatform.Contracts.Events;
using Confluent.Kafka;
using SupplierIntegration.Worker.Clients;
using SupplierIntegration.Worker.Messaging;

namespace SupplierIntegration.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly ISupplierClient _supplierClient;
    private readonly IDeadLetterPublisher _deadLetterPublisher;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, ISupplierClient supplierClient, IDeadLetterPublisher deadLetterPublisher)
    {
        _logger = logger;
        _configuration = configuration;
        _supplierClient = supplierClient;
        _deadLetterPublisher = deadLetterPublisher;
    }   

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await Task.Yield();

        var bootstrapServers = _configuration.GetValue<string>("Kafka:BootstrapServers") 
            ?? throw new InvalidOperationException("Kafka bootstrap servers configuration is missing.");    

        var groupId = _configuration.GetValue<string>("Kafka:GroupId") 
            ?? throw new InvalidOperationException("Kafka group ID configuration is missing.");

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = groupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe("booking-created-v1");

        _logger.LogInformation("Worker started. Listening for booking-created-v1 events...");   

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ConsumeResult<string, string> result;
    
                try
                {
                    result = consumer.Consume(cancellationToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error occurred while consuming message from Kafka.");
                    await Task.Delay(5000, cancellationToken);
                    continue;
                }
    
                try
                {
                    var bookingCreatedEvent = JsonSerializer.Deserialize<BookingCreated>(result.Message.Value);
    
                    if (bookingCreatedEvent is null)
                    {
                        throw new JsonException("Booking created event deserialized to null");
                    }
                
                    _logger.LogInformation("Received booking created event: {BookingId}, Hotel: {HotelName}", bookingCreatedEvent.BookingId, bookingCreatedEvent?.HotelName);    
    
                    //await Task.Delay(1000, stoppingToken);
                    var supplierBookingRequest = new SupplierBookingRequest(
                        bookingCreatedEvent.BookingId,
                        bookingCreatedEvent.CustomerName,
                        bookingCreatedEvent.HotelName,
                        bookingCreatedEvent.CheckInDate,
                        bookingCreatedEvent.CheckOutDate
                    );
    
                    var supplierBookingResponse = await _supplierClient.CreateBookingAsync(supplierBookingRequest, cancellationToken);
    
                    _logger.LogInformation("Supplier booking response: BookingId: {BookingId}, SupplierReference: {SupplierReference}, Status: {Status}", 
                        supplierBookingResponse.BookingId, 
                        supplierBookingResponse.SupplierReference, 
                        supplierBookingResponse.Status);
    
    
                    _logger.LogInformation("Processed booking created event: {BookingId}", bookingCreatedEvent?.BookingId);
    
                    consumer.Commit(result);
                }
                catch (HttpRequestException ex)
                {
                    await PublishToDeadLetterAndCommitAsync(consumer, result, ex, cancellationToken); 
                }
                catch (JsonException ex)
                {
                    await PublishToDeadLetterAndCommitAsync(consumer, result, ex, cancellationToken); 
                }
            }
        }    
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Worker is stopping due to cancellation.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing booking events.");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Worker stopped.");
        }
    }

    private async Task PublishToDeadLetterAndCommitAsync(
        IConsumer<string, string> consumer,
        ConsumeResult<string, string> result,
        Exception exception,
        CancellationToken cancellationToken)
    {
        await _deadLetterPublisher.PublishAsync(result, exception, cancellationToken);

           _logger.LogWarning(
                "Published message to DLQ. Source topic: {Topic}, partition: {Partition}, offset: {Offset}",
                result.Topic,
                result.Partition.Value,
                result.Offset.Value);

        consumer.Commit(result);
    }
}
