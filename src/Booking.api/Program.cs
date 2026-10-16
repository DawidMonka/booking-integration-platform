using BookingPlatform.Api.Messaging;
using BookingPlatform.Domain;
using BookingPlatform.Domain.Repositories;
using BookingPlatform.Infrastructure.Persistence;
using BookingPlatform.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using BookingPlatform.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("BookingDatabase")
    ?? throw new InvalidOperationException(
        "Connection string 'BookingDatabase' was not found.");

builder.Services.AddDbContext<BookingDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<IBookingRepository, EfBookingRepository>();

builder.Services.AddSingleton<IEventPublisher, KafkaEventPublisher>();
builder.Services.AddScoped<IBookingCreationService, BookingCreationService>();
builder.Services.AddHostedService<OutboxPublisherService>();

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
