namespace MockSupplier.Api.Contracts;

public sealed class SupplierBookingRequest(Guid BookingId, string CustomerName, string HotelName, DateOnly CheckInDate, DateOnly CheckOutDate)
{
    public Guid BookingId { get; set; } = BookingId;
    public string CustomerName { get; set; } = CustomerName;
    public string HotelName { get; set; } = HotelName;
    public DateOnly CheckInDate { get; set; } = CheckInDate;
    public DateOnly CheckOutDate { get; set; } = CheckOutDate;
}