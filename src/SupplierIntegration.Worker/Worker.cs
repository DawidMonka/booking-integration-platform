using System.Text.Json;
using BookingPlatform.Contracts.Events;
using Confluent.Kafka;
using SupplierIntegration.Worker.Clients;

namespace SupplierIntegration.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly ISupplierClient _supplierClient;


    public Worker(ILogger<Worker> logger, IConfiguration configuration, ISupplierClient supplierClient)
    {
        _logger = logger;
        _configuration = configuration;
        _supplierClient = supplierClient;
    }   

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var bookingCreatedEvent = JsonSerializer.Deserialize<BookingCreated>(result.Message.Value);

                    if (bookingCreatedEvent is null)
                    {
                        _logger.LogWarning(
                            "Received invalid booking created event at offset {Offset}",
                            result.Offset);

                        continue;
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

                    var supplierBookingResponse = await _supplierClient.CreateBookingAsync(supplierBookingRequest, stoppingToken);

                    _logger.LogInformation("Supplier booking response: BookingId: {BookingId}, SupplierReference: {SupplierReference}, Status: {Status}", 
                        supplierBookingResponse.BookingId, 
                        supplierBookingResponse.SupplierReference, 
                        supplierBookingResponse.Status);


                    _logger.LogInformation("Processed booking created event: {BookingId}", bookingCreatedEvent?.BookingId);

                    consumer.Commit(result);
                }
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, "Error occurred while calling the supplier API.");

                    await Task.Delay(5000, stoppingToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error occurred while consuming message from Kafka.");

                    await Task.Delay(5000, stoppingToken);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Error occurred while deserializing booking created event.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error occurred while processing booking created event.");
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
}
