using BookingPlatform.Domain;
using BookingPlatform.Domain.Repositories;
using BookingPlatform.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();

builder.Services.AddOpenApi();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//pp.UseHttpsRedirection();

app.MapControllers();

app.Run();
