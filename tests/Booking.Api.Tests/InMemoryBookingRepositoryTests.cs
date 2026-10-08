using BookingPlatform.Domain;
using BookingPlatform.Infrastructure.Repositories;

namespace BookingPlatform.Tests;

public sealed class InMemoryBookingRepositoryTests
{
    [Fact]
    public async Task Add_ShouldStoreBooking()
    {
        // Arrange
        var repository = new InMemoryBookingRepository();

        var booking = new Booking(
            "John Doe",
            "Grand Hotel",
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 15));

        // Act
        await repository.AddAsync(booking);

        // Assert
        var result = await repository.GetByIdAsync(booking.Id);

        Assert.NotNull(result);
        Assert.Equal(booking.Id, result.Id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenBookingDoesNotExist()
    {
        var repository = new InMemoryBookingRepository();

        var result = await repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAddedBookings()
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

        await repository.AddAsync(booking1);
        await repository.AddAsync(booking2);

        var result = await repository.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == booking1.Id);
        Assert.Contains(result, x => x.Id == booking2.Id);
    }
}