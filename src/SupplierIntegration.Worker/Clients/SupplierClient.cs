using System.Net.Http.Json;
using System.Net;

namespace SupplierIntegration.Worker.Clients;

public sealed class SupplierClient : ISupplierClient
{
    private const int MaxRetries = 3;
    private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(2);

    private readonly HttpClient _httpClient;
    private readonly ILogger<SupplierClient> _logger;

    public SupplierClient(HttpClient httpClient, ILogger<SupplierClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;   
    }

    public async Task<SupplierBookingResponse> CreateBookingAsync(SupplierBookingRequest request, CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                using var response = await _httpClient.PostAsJsonAsync("api/supplier/bookings", request, cancellationToken);

                if (IsTransient(response.StatusCode) && attempt < MaxRetries)
                {
                    var delay = GetBackoff(attempt);

                    _logger.LogWarning(
                        "Transient error occurred while calling the supplier API. Status code: {StatusCode}. Retrying in {Delay} seconds (Attempt {Attempt}/{MaxRetries})",
                        response.StatusCode, delay.TotalSeconds, attempt, MaxRetries);

                    await Task.Delay(delay, cancellationToken);
                    continue;
                }

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<SupplierBookingResponse>(cancellationToken: cancellationToken);

                return result ?? throw new InvalidOperationException("Response content is null"); 
            }
            catch (HttpRequestException ex) when (attempt < MaxRetries)
            {
                var delay = GetBackoff(attempt);

                _logger.LogWarning(
                    ex,
                    "Transient error occurred while calling the supplier API. Retrying in {Delay} seconds (Attempt {Attempt}/{MaxRetries})",
                    delay.TotalSeconds, attempt, MaxRetries);

                await Task.Delay(delay, cancellationToken);
            }
        }

        throw new InvalidOperationException("Failed to create booking after multiple attempts.");
    }

    private static TimeSpan GetBackoff(int attempt)
    {   
        var expotencialDelay = InitialDelay.TotalMilliseconds * Math.Pow(2, attempt - 1);
        var jitter = new Random().NextDouble() * 1000;
        
        return TimeSpan.FromMilliseconds(expotencialDelay + jitter);
    }

    private static bool IsTransient(HttpStatusCode statusCode) => 
    statusCode == HttpStatusCode.RequestTimeout || statusCode == HttpStatusCode.TooManyRequests || (int)statusCode >= 500;

}