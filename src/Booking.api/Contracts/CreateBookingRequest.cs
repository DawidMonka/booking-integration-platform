namespace Booking.Api.Contracts
{
    public sealed record CreateBookingRequest
    {
        public string CustomerName { get; set; }
        public string HotelName { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }

        public CreateBookingRequest(string customerName, string hotelName, DateOnly checkInDate, DateOnly checkOutDate)
        {
            CustomerName = customerName;
            HotelName = hotelName;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
        }
    }
}