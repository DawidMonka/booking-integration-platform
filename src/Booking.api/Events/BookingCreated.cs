namespace BookingPlatform.Api.Events;

public sealed record BookingCreated(Guid BookingId, string CustomerName, string HotelName, DateOnly CheckInDate, DateOnly CheckOutDate, DateTimeOffset CreatedAt);


