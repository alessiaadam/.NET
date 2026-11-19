using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever(); // the domain generates the Guid

        builder.Property(b => b.RoomId).IsRequired();
        builder.HasOne<Room>().WithMany().HasForeignKey(b => b.RoomId).OnDelete(DeleteBehavior.Cascade);

        builder.Property(b => b.BookedBy).IsRequired().HasMaxLength(200);
        builder.Property(b => b.Attendees).IsRequired();

        // Enum stored as text ("Active" / "Cancelled").
        builder.Property(b => b.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        // Value object DateRange -> two columns on the Bookings table, always UTC.
        builder.ComplexProperty(b => b.Period, period =>
        {
            period.Property(p => p.StartUtc).HasColumnName("StartUtc").HasConversion<UtcDateTimeConverter>();
            period.Property(p => p.EndUtc).HasColumnName("EndUtc").HasConversion<UtcDateTimeConverter>();
        });

        builder.Ignore(b => b.IsActive);

        builder.HasIndex(b => new { b.RoomId, b.Status });
    }
}
