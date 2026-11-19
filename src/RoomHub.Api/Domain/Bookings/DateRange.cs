using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

// Value object: a half-open UTC interval [StartUtc, EndUtc).
public sealed record DateRange
{
    // For EF Core materialization only.
    private DateRange()
    {
    }

    private DateRange(DateTime startUtc, DateTime endUtc)
    {
        StartUtc = startUtc;
        EndUtc = endUtc;
    }

    public DateTime StartUtc { get; private init; }
    public DateTime EndUtc { get; private init; }

    public TimeSpan Duration => EndUtc - StartUtc;

    public static Result<DateRange> Create(DateTime? startUtc, DateTime? endUtc)
    {
        if (startUtc is null || endUtc is null)
            return BookingErrors.IntervalRequired;

        var start = ToUtc(startUtc.Value);
        var end = ToUtc(endUtc.Value);

        // R3: end strictly after start.
        if (end <= start)
            return BookingErrors.EndNotAfterStart;

        return new DateRange(start, end);
    }

    // R7: half-open intervals. Touching (this.End == other.Start) is NOT an overlap.
    public bool Overlaps(DateRange other) =>
        StartUtc < other.EndUtc && other.StartUtc < EndUtc;

    // "+02:00" offsets become UTC; a date without an offset is treated as UTC.
    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
