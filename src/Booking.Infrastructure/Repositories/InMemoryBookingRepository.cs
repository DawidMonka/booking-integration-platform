using System.Collections.Concurrent;
using BookingPlatform.Domain.Repositories;
using BookingPlatform.Domain;
using System.Collections.Generic;

namespace BookingPlatform.Infrastructure.Repositories;

public sealed class InMemoryBookingRepository : IBookingRepository
{
    private readonly ConcurrentDictionary<Guid, Booking> _bookings = new();

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _bookings.TryGetValue(id, out var booking);

        return Task.FromResult(booking);
    }

    public Task AddAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_bookings.TryAdd(booking.Id, booking))
        {
            throw new InvalidOperationException($"Booking with ID {booking.Id} already exists.");
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IReadOnlyCollection<Booking> bookings = _bookings.Values.ToArray();

        return Task.FromResult(bookings);
    }
}