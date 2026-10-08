using BookingPlatform.Domain.Repositories;
using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Domain;

namespace BookingPlatform.Infrastructure.Repositories;

public sealed class EfBookingRepository : IBookingRepository
{
    private readonly BookingDbContext _dbContext;

    public EfBookingRepository(BookingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _dbContext.Bookings.AddAsync(booking, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _dbContext.Bookings.AsNoTracking().ToListAsync(cancellationToken);
    }
}