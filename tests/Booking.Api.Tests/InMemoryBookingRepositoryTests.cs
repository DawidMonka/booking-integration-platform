using BookingPlatform.Domain;
using BookingPlatform.Infrastructure.Repositories;

namespace BookingPlatform.Tests;

public sealed class InMemoryBookingRepositoryTests
{
    [Fact]
    public void Add_ShouldStoreBooking()
    {
        // Arrange
        var repository = new InMemoryBookingRepository();

        var booking = new Booking(
            "John Doe",
            "Grand Hotel",
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 15));

        // Act
        repository.Add(booking);

        // Assert
        var result = repository.GetById(booking.Id);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenBookingDoesNotExist()
    {
        var repository = new InMemoryBookingRepository();

        var result = repository.GetById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public void GetAll_ShouldReturnAddedBookings()
    {
        var repository = new InMemoryBookingRepository();

        var booking1 = new Booking(
            "John Doe",
            "Hotel One",
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 15));

        var booking2 = new Booking(
            "Jane Doe",
            "Hotel Two",
            new DateOnly(2026, 12, 1),
            new DateOnly(2026, 12, 5));

        repository.Add(booking1);
        repository.Add(booking2);

        var result = repository.GetAll().ToList();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == booking1.Id);
        Assert.Contains(result, x => x.Id == booking2.Id);
    }
}