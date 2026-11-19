using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed record CreateBookingCommand(
    Guid RoomId, string? BookedBy, DateTime? StartUtc, DateTime? EndUtc, int? Attendees) : ICommand<Booking>;

// Depends only on domain interfaces (no EF Core here).
public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICommandHandler<CreateBookingCommand, Booking>
{
    public async Task<Result<Booking>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        // Contract: 404 for a missing room, before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
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
        if (await bookings.HasActiveOverlapAsync(room.Id, booking.Value.Period, cancellationToken))
            return BookingErrors.Overlap;

        bookings.Add(booking.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return booking;
    }
}
