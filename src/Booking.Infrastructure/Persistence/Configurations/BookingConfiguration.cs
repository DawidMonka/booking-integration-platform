using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BookingPlatform.Domain;

namespace BookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.CustomerName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.HotelName).IsRequired().HasMaxLength(200);
        builder.Property(b => b.CheckInDate).IsRequired();
        builder.Property(b => b.CheckOutDate).IsRequired();
        builder.Property(b => b.Status).IsRequired();
        builder.Property(b => b.CreatedAt).IsRequired();
    }
}