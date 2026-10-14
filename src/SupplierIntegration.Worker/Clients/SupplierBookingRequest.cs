namespace SupplierIntegration.Worker.Clients;

public sealed class SupplierBookingRequest(Guid bookingId, string customerName, string hotelName, DateOnly checkInDate, DateOnly checkOutDate)
{
    public Guid BookingId { get; set; } = bookingId;
    public string CustomerName { get; set; } = customerName;
    public string HotelName { get; set; } = hotelName;
    public DateOnly CheckInDate { get; set; } = checkInDate;
    public DateOnly CheckOutDate { get; set; } = checkOutDate;
}