namespace MockSupplier.Api.Contracts;

public sealed class SupplierBookingResponse(Guid BookingId, string SupplierReference, string Status)
{
    public Guid BookingId { get; set; } = BookingId;
    public string SupplierReference { get; set; } = SupplierReference;
    public string Status { get; set; } = Status;
} 