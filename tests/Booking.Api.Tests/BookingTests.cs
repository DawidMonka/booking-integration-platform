using Xunit;
using BookingPlatform.Domain;

namespace BookingPlatform.Tests;

public sealed class BookingTests
{
    [Fact]
    public void CreateBooking_ShouldInitializeProperties()
    {
        // Arrange
        var customerName = "John Doe";
        var hotelName = "Grand Hotel";
        var checkInDate = new DateOnly(2023, 10, 1);
        var checkOutDate = new DateOnly(2023, 10, 5);

        // Act
        var booking = new Booking(customerName, hotelName, checkInDate, checkOutDate);

        // Assert
        Assert.Equal(customerName, booking.CustomerName);
        Assert.Equal(hotelName, booking.HotelName);
        Assert.Equal(checkInDate, booking.CheckInDate);
        Assert.Equal(checkOutDate, booking.CheckOutDate);
        Assert.Equal(BookingStatus.Pending, booking.Status);
        Assert.True(booking.CreatedAt <= DateTimeOffset.UtcNow);
        Assert.NotEqual(Guid.Empty, booking.Id);
    }

    [Fact]
    public void CreateBooking_ShouldThrowArgumentException_WhenCustomerNameIsEmpty()
    {
        // Arrange
        var customerName = "";
        var hotelName = "Grand Hotel";
        var checkInDate = new DateOnly(2023, 10, 1);
        var checkOutDate = new DateOnly(2023, 10, 5);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Domain.Booking(customerName, hotelName, checkInDate, checkOutDate));
    }

    [Fact]
    public void CreateBooking_ShouldThrowArgumentException_WhenHotelNameIsEmpty()
    {
        // Arrange
        var customerName = "John Doe";
        var hotelName = "";
        var checkInDate = new DateOnly(2023, 10, 1);
        var checkOutDate = new DateOnly(2023, 10, 5);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Domain.Booking(customerName, hotelName, checkInDate, checkOutDate));
    }

    [Fact]
    public void CreateBooking_ShouldThrowArgumentException_WhenCheckInDateIsAfterCheckOutDate()
    {
        // Arrange
        var customerName = "John Doe";
        var hotelName = "Grand Hotel";
        var checkInDate = new DateOnly(2023, 10, 5);
        var checkOutDate = new DateOnly(2023, 10, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Domain.Booking(customerName, hotelName, checkInDate, checkOutDate));
    }

    [Fact]
    public void CreateBooking_ShouldThrowArgumentException_WhenCheckInDateIsEqualToCheckOutDate()
    {
        // Arrange
        var customerName = "John Doe";
        var hotelName = "Grand Hotel";
        var checkInDate = new DateOnly(2023, 10, 1);
        var checkOutDate = new DateOnly(2023, 10, 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Domain.Booking(customerName, hotelName, checkInDate, checkOutDate));
    }
}