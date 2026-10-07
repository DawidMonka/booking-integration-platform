public sealed class Booking
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; }
    public string HotelName { get; private set; }
    public DateOnly CheckInDate { get; private set; }
    public DateOnly CheckOutDate { get; private set; }
    public BookingStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public Booking(string customerName, string hotelName, DateOnly checkInDate, DateOnly checkOutDate)
    {
if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name cannot be null or empty.", nameof(customerName));
        }

        if (string.IsNullOrWhiteSpace(hotelName))
        {
            throw new ArgumentException("Hotel name cannot be null or empty.", nameof(hotelName));
        }

        if (checkInDate >= checkOutDate)
        {
            throw new ArgumentException("Check-in date must be before check-out date.");
        }

        Id = Guid.NewGuid();
        CustomerName = customerName;
        HotelName = hotelName;
        CheckInDate = checkInDate;
        CheckOutDate = checkOutDate;
        Status = Status.Pending;
        CreatedAt = DateTime.UtcNow;
    }
}