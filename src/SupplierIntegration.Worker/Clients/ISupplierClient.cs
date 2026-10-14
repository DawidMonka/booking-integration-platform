namespace SupplierIntegration.Worker.Clients;

public interface ISupplierClient
{
    Task<SupplierBookingResponse> CreateBookingAsync(SupplierBookingRequest request, CancellationToken cancellationToken);
}