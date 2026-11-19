using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId, string? BookedBy, DateTime? StartUtc, DateTime? EndUtc, int? Attendees) : ICommand<Booking>;

public sealed class CreateBookingHandler(
    RoomRepository rooms,
    BookingRepository bookings,
    TimeProvider timeProvider) : ICommandHandler<CreateBookingCommand, Booking>
{
    public Task<Result<Booking>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
        => Task.FromResult(Execute(command));

    private Result<Booking> Execute(CreateBookingCommand command)
    {
        // Contract: 404 for a missing room, before any validation.
        var room = rooms.Get(command.RoomId);
        if (room is null)
            return RoomErrors.NotFound;

        // R2-R6 live in the domain. "Now" comes only from TimeProvider (R5).
        var booking = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            timeProvider.GetUtcNow().UtcDateTime);

        if (booking.IsFailure)
            return booking;

        // R7: needs the other bookings of the room.
        if (bookings.HasActiveOverlap(room.Id, booking.Value.Period))
            return BookingErrors.Overlap;

        bookings.Add(booking.Value);
        return booking;
    }
}
