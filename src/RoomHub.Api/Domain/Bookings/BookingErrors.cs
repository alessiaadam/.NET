using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Domain.Bookings;

public static class BookingErrors
{
    public static readonly Error BookedByRequired =
        Error.Validation("Booking.BookedByRequired", "BookedBy is required.");

    public static readonly Error IntervalRequired =
        Error.Validation("Booking.IntervalRequired", "StartUtc and EndUtc are required.");

    public static readonly Error EndNotAfterStart =
        Error.Validation("Booking.EndNotAfterStart", "The end must be strictly after the start.");

    public static readonly Error StartInPast =
        Error.Validation("Booking.StartInPast", "A booking cannot start in the past.");

    public static readonly Error TooLong =
        Error.Validation("Booking.TooLong", "A booking can last at most 4 hours.");

    public static readonly Error InvalidAttendees =
        Error.Validation("Booking.InvalidAttendees", "Attendees must be strictly positive.");

    public static readonly Error AttendeesExceedCapacity =
        Error.Validation("Booking.AttendeesExceedCapacity", "The room does not have enough seats.");

    public static readonly Error Overlap =
        Error.Conflict("Booking.Overlap", "The room already has an active booking in this interval.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Booking.AlreadyCancelled", "The booking is already cancelled.");

    public static readonly Error NotFound =
        Error.NotFound("Booking.NotFound", "The booking does not exist.");
}
