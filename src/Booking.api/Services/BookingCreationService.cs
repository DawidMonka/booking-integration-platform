using System.Text.Json;
using BookingPlatform.Contracts.Events;
using BookingPlatform.Domain;
using BookingPlatform.Infrastructure.Persistence;

namespace BookingPlatform.Api.Services;

public sealed class BookingCreationService(BookingDbContext dbContext) : IBookingCreationService
{
    public async Task CreateAsync(Booking booking, CancellationToken cancellationToken)
    {
        var bookingCreatedEvent = new BookingCreated(
            booking.Id,
            booking.CustomerName,
            booking.HotelName,
            booking.CheckInDate,
            booking.CheckOutDate,
            booking.CreatedAt
        );

        var payload = JsonSerializer.Serialize(bookingCreatedEvent);

        var outboxMessage = new OutboxMessage(booking.Id, "booking-created-v1", payload);

        dbContext.Bookings.Add(booking);
        dbContext.OutboxMessages.Add(outboxMessage);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
