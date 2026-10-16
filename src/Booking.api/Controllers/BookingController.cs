using BookingPlatform.Domain;
using BookingPlatform.Domain.Repositories;
using BookingPlatform.Api.Contracts;
using Microsoft.AspNetCore.Mvc;
using BookingPlatform.Api.Services;

namespace BookingPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BookingsController(IBookingRepository bookingRepository, IBookingCreationService bookingCreationService) : ControllerBase
{
    private readonly IBookingRepository _bookingRepository = bookingRepository;
    private readonly IBookingCreationService _bookingCreationService = bookingCreationService;

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
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var booking = new Booking(request.CustomerName, request.HotelName, request.CheckInDate, request.CheckOutDate);
            
            await _bookingCreationService.CreateAsync(booking, cancellationToken);

            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

}