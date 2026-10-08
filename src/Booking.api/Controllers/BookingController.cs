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
    public IActionResult GetAllBookings()
    {
        var bookings = _bookingRepository.GetAll();
        return Ok(bookings);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public IActionResult GetBookingById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest("Invalid booking ID.");
        }

        var booking = _bookingRepository.GetById(id);
        if (booking == null)
        {
            return NotFound();
        }
        return Ok(booking);
    }

    [HttpPost]
    public IActionResult CreateBooking([FromBody] CreateBookingRequest request)
    {
        try
        {
            var booking = new Booking(request.CustomerName, request.HotelName, request.CheckInDate, request.CheckOutDate);
            
            _bookingRepository.Add(booking);
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}