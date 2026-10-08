namespace BookingPlatform.Domain.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Booking>> GetAllAsync(CancellationToken cancellationToken = default);
}