using System.Collections.Concurrent;
using BookingPlatform.Domain.Repositories;
using BookingPlatform.Domain;

namespace BookingPlatform.Infrastructure.Repositories;

public sealed class InMemoryBookingRepository : IBookingRepository
{
    private readonly ConcurrentDictionary<Guid, Booking> _bookings = new();

    public Booking? GetById(Guid id)
    {
        return _bookings.TryGetValue(id, out var booking) ? booking : null;
    }

    public void Add(Booking booking)
    {
        if (!_bookings.TryAdd(booking.Id, booking))
        {
            throw new InvalidOperationException($"A booking with ID {booking.Id} already exists.");
        }
    }

    public IEnumerable<Booking> GetAll()
    {
        return _bookings.Values;
    }
}