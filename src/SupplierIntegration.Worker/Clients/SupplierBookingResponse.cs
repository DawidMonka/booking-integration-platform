namespace SupplierIntegration.Worker.Clients;

public sealed class SupplierBookingResponse(Guid bookingId, string supplierReference, string status)
{
    public Guid BookingId { get; set; } = bookingId;
    public string SupplierReference { get; set; } = supplierReference;
    public string Status { get; set;} = status;
}