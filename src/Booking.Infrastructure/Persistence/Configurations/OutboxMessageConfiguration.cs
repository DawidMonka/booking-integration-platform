using BookingPlatform.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(message => message.Id);

        builder.Property(message => message.AggregateId)
            .IsRequired();

        builder.Property(message => message.EventType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(message => message.Payload)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(message => message.OccurredAtUtc)
            .IsRequired();

        builder.Property(message => message.PublishedAtUtc);

        builder.Property(message => message.AttemptCount)
            .IsRequired();

        builder.Property(message => message.LastError)
            .HasMaxLength(4000);

        builder.HasIndex(message => new
            {
                message.PublishedAtUtc,
                message.OccurredAtUtc
            });
    }
}