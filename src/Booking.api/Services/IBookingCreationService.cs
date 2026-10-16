using BookingPlatform.Domain;

namespace BookingPlatform.Api.Services;

public interface IBookingCreationService
{
    Task CreateAsync(Booking booking, CancellationToken cancellationToken);
}