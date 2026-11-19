using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Domain.Bookings;

public enum BookingStatus
{
    Active,
    Cancelled
}

public sealed class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    // For EF Core materialization only.
    private Booking()
    {
        BookedBy = null!;
        Period = null!;
    }

    private Booking(Guid id, Guid roomId, string bookedBy, DateRange period, int attendees)
    {
        Id = id;
        RoomId = roomId;
        BookedBy = bookedBy;
        Period = period;
        Attendees = attendees;
        Status = BookingStatus.Active;
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; }
    public DateRange Period { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;

    // R2-R6. nowUtc is passed in (it comes from TimeProvider), so the domain never reads the clock.
    // R7 needs the other bookings of the room, so it is checked by the handler through the repository.
    public static Result<Booking> Create(
        Room room, string? bookedBy, DateTime? startUtc, DateTime? endUtc, int? attendees, DateTime nowUtc)
    {
        // R2
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.BookedByRequired;

        // R3
        var period = DateRange.Create(startUtc, endUtc);
        if (period.IsFailure)
            return period.Error!;

        // R5: starting exactly "now" is allowed.
        if (period.Value.StartUtc < nowUtc)
            return BookingErrors.StartInPast;

        // R6: exactly 4 hours is allowed.
        if (period.Value.Duration > MaxDuration)
            return BookingErrors.TooLong;

        // R4
        if (attendees is null or <= 0)
            return BookingErrors.InvalidAttendees;

        if (attendees > room.Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        return new Booking(Guid.NewGuid(), room.Id, bookedBy.Trim(), period.Value, attendees.Value);
    }

    // R8: Active -> Cancelled; cancelling twice is a conflict.
    public Result<Unit> Cancel()
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        Status = BookingStatus.Cancelled;
        return Unit.Value;
    }

    // R7: only active bookings block an interval.
    public bool ConflictsWith(DateRange period) => IsActive && Period.Overlaps(period);
}
