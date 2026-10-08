using BookingPlatform.Domain;
using BookingPlatform.Domain.Repositories;
using BookingPlatform.Api.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace BookingPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingsController : ControllerBase
{
    private readonly IBookingRepository _bookingRepository;

    public BookingsController(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _bookingRepository.GetAllAsync();
        return Ok(bookings);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetBookingById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid booking ID.");
        }

        var booking = await _bookingRepository.GetByIdAsync(id);
        if (booking == null)
        {
            return NotFound();
        }
        return Ok(booking);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var booking = new Booking(request.CustomerName, request.HotelName, request.CheckInDate, request.CheckOutDate);
            
            await _bookingRepository.AddAsync(booking);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}