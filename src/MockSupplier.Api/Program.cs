using MockSupplier.Api.Contracts;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/supplier/bookings", (SupplierBookingRequest request) =>
{
    if (request.HotelName == "Fail Hotel")
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    var response = new SupplierBookingResponse(
        BookingId: request.BookingId,
        SupplierReference: "SUP-" + request.BookingId.ToString().Substring(0, 8),
        Status: "Confirmed"
    );

    return Results.Ok(response);
});

app.Run();
